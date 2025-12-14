using System.Reflection;
using EventBus.Core.Enums;

namespace EventBus.Core.Models;

/// <summary>
/// Encapsulates information about a subscriber's event handler method.
/// </summary>
public class SubscriberMethod
{
    /// <summary>
    /// Gets the subscriber instance that owns this handler method.
    /// </summary>
    public object Subscriber { get; }
    
    /// <summary>
    /// Gets the reflection information about the handler method.
    /// </summary>
    public MethodInfo Method { get; }
    
    /// <summary>
    /// Gets the type of event this handler accepts.
    /// </summary>
    public Type EventType { get; }
    
    /// <summary>
    /// Gets the execution priority of this handler.
    /// </summary>
    public int Priority { get; }
    
    /// <summary>
    /// Gets the threading behavior for this handler.
    /// </summary>
    public ThreadMode ThreadMode { get; }
    
    public SubscriberMethod(object subscriber, MethodInfo method, Type eventType, int priority, ThreadMode threadMode)
    {
        Subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
        Method = method ?? throw new ArgumentNullException(nameof(method));
        EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
        Priority = priority;
        ThreadMode = threadMode;
    }
    
    /// <summary>
    /// Invokes the handler method with the specified event.
    /// </summary>
    public void Invoke(object eventObject)
    {
        if (eventObject == null)
            throw new ArgumentNullException(nameof(eventObject));
            
        Method.Invoke(Subscriber, new[] { eventObject });
    }
    
    /// <summary>
    /// Invokes the handler method asynchronously with the specified event.
    /// </summary>
    public async Task InvokeAsync(object eventObject)
    {
        if (eventObject == null)
            throw new ArgumentNullException(nameof(eventObject));
            
        var result = Method.Invoke(Subscriber, new[] { eventObject });
        
        // If the method returns a Task, await it
        if (result is Task task)
        {
            await task;
        }
    }
}
