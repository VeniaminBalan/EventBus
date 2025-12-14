using System.Reflection;
using EventBus.Core.Attributes;
using EventBus.Core.Enums;
using EventBus.Core.Exceptions;
using EventBus.Core.Models;
using EventBus.Core.Registry;

namespace EventBus.Core;

/// <summary>
/// Central hub for managing event publishing and subscription.
/// </summary>
public class EventBus
{
    private static readonly Lazy<EventBus> _default = new(() => new EventBus());
    
    /// <summary>
    /// Gets the default singleton instance of EventBus.
    /// </summary>
    public static EventBus Default => _default.Value;
    
    private readonly SubscriberRegistry _registry;
    private readonly EventBusConfiguration _configuration;
    private readonly object _publishLock = new();
    
    /// <summary>
    /// Creates a new instance of EventBus with default configuration.
    /// </summary>
    public EventBus() : this(new EventBusConfiguration())
    {
    }
    
    /// <summary>
    /// Creates a new instance of EventBus with the specified configuration.
    /// </summary>
    public EventBus(EventBusConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _registry = new SubscriberRegistry();
    }
    
    /// <summary>
    /// Registers an object as a subscriber. Scans for methods with [EventHandler] attribute.
    /// </summary>
    public void Register(object subscriber)
    {
        if (subscriber == null)
            throw new ArgumentNullException(nameof(subscriber));
            
        var type = subscriber.GetType();
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<EventHandlerAttribute>();
            if (attribute == null)
                continue;
                
            ValidateHandlerMethod(method);
            
            var parameters = method.GetParameters();
            var eventType = parameters[0].ParameterType;
            
            var subscriberMethod = new SubscriberMethod(
                subscriber,
                method,
                eventType,
                attribute.Priority,
                attribute.ThreadMode
            );
            
            _registry.AddHandler(subscriberMethod);
        }
    }
    
    /// <summary>
    /// Unregisters a subscriber, removing all its event handlers.
    /// </summary>
    public void Unregister(object subscriber)
    {
        if (subscriber == null)
            throw new ArgumentNullException(nameof(subscriber));
            
        _registry.RemoveHandler(subscriber);
    }
    
    /// <summary>
    /// Publishes an event to all registered handlers synchronously.
    /// </summary>
    public void Publish(object eventObject)
    {
        if (eventObject == null)
            throw new ArgumentNullException(nameof(eventObject));
            
        // Avoid recursive publishing of SubscriberExceptionEvent or DeadEvent
        if (eventObject is SubscriberExceptionEvent || eventObject is DeadEvent)
        {
            PublishDirect(eventObject);
            return;
        }
            
        var eventType = eventObject.GetType();
        var handlers = _registry.GetHandlers(eventType).ToList();
        
        if (!handlers.Any())
        {
            HandleNoSubscribers(eventObject);
            return;
        }
        
        foreach (var handler in handlers)
        {
            InvokeHandler(handler, eventObject);
        }
    }
    
    /// <summary>
    /// Publishes an event to all registered handlers asynchronously.
    /// </summary>
    public async Task PublishAsync(object eventObject)
    {
        if (eventObject == null)
            throw new ArgumentNullException(nameof(eventObject));
            
        // Avoid recursive publishing of SubscriberExceptionEvent or DeadEvent
        if (eventObject is SubscriberExceptionEvent || eventObject is DeadEvent)
        {
            PublishDirect(eventObject);
            return;
        }
            
        var eventType = eventObject.GetType();
        var handlers = _registry.GetHandlers(eventType).ToList();
        
        if (!handlers.Any())
        {
            HandleNoSubscribers(eventObject);
            return;
        }
        
        var tasks = new List<Task>();
        
        foreach (var handler in handlers)
        {
            if (handler.ThreadMode == ThreadMode.Async)
            {
                tasks.Add(InvokeHandlerAsync(handler, eventObject));
            }
            else
            {
                InvokeHandler(handler, eventObject);
            }
        }
        
        if (tasks.Any())
        {
            await Task.WhenAll(tasks);
        }
    }
    
    private void PublishDirect(object eventObject)
    {
        var eventType = eventObject.GetType();
        var handlers = _registry.GetHandlers(eventType).ToList();
        
        foreach (var handler in handlers)
        {
            try
            {
                handler.Invoke(eventObject);
            }
            catch
            {
                // Silently ignore exceptions in exception/dead event handlers
            }
        }
    }
    
    private void InvokeHandler(SubscriberMethod handler, object eventObject)
    {
        try
        {
            switch (handler.ThreadMode)
            {
                case ThreadMode.Posting:
                    handler.Invoke(eventObject);
                    break;
                    
                case ThreadMode.Background:
                    Task.Run(() => handler.Invoke(eventObject));
                    break;
                    
                case ThreadMode.Async:
                    // Will be handled in PublishAsync
                    handler.Invoke(eventObject);
                    break;
                    
                case ThreadMode.MainThread:
                    // For now, just invoke on current thread
                    // In a real implementation, this would use SynchronizationContext
                    handler.Invoke(eventObject);
                    break;
            }
        }
        catch (Exception ex)
        {
            HandleSubscriberException(ex, eventObject, handler.Subscriber);
        }
    }
    
    private async Task InvokeHandlerAsync(SubscriberMethod handler, object eventObject)
    {
        try
        {
            await handler.InvokeAsync(eventObject);
        }
        catch (Exception ex)
        {
            HandleSubscriberException(ex, eventObject, handler.Subscriber);
        }
    }
    
    private void HandleSubscriberException(Exception exception, object eventObject, object subscriber)
    {
        // Unwrap TargetInvocationException
        var actualException = exception is TargetInvocationException tie && tie.InnerException != null
            ? tie.InnerException
            : exception;
            
        if (_configuration.LogSubscriberExceptions)
        {
            Console.WriteLine($"EventBus: Exception in subscriber: {actualException}");
        }
        
        if (_configuration.SendSubscriberExceptionEvent)
        {
            var exceptionEvent = new SubscriberExceptionEvent(this, actualException, eventObject, subscriber);
            PublishDirect(exceptionEvent);
        }
        
        if (_configuration.ThrowSubscriberException)
        {
            throw new EventBusException("Exception in event handler", actualException);
        }
    }
    
    private void HandleNoSubscribers(object eventObject)
    {
        if (_configuration.LogNoSubscriberMessages)
        {
            Console.WriteLine($"EventBus: No subscribers for event type: {eventObject.GetType().Name}");
        }
        
        if (_configuration.SendNoSubscriberEvent)
        {
            var deadEvent = new DeadEvent(eventObject, this);
            PublishDirect(deadEvent);
        }
    }
    
    private void ValidateHandlerMethod(MethodInfo method)
    {
        var parameters = method.GetParameters();
        
        if (parameters.Length != 1)
        {
            throw new EventBusException(
                $"Event handler method {method.Name} must have exactly one parameter. Found {parameters.Length} parameters."
            );
        }
        
        if (parameters[0].ParameterType.IsPrimitive)
        {
            throw new EventBusException(
                $"Event handler method {method.Name} parameter cannot be a primitive type."
            );
        }
    }
}
