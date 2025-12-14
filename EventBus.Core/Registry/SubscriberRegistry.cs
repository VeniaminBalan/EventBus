using System.Collections.Concurrent;

namespace EventBus.Core.Registry;

/// <summary>
/// Thread-safe registry that maps event types to their subscriber methods.
/// </summary>
public class SubscriberRegistry
{
    private readonly ConcurrentDictionary<Type, ConcurrentBag<Models.SubscriberMethod>> _handlers;
    
    public SubscriberRegistry()
    {
        _handlers = new ConcurrentDictionary<Type, ConcurrentBag<Models.SubscriberMethod>>();
    }
    
    /// <summary>
    /// Adds a handler method for a specific event type.
    /// </summary>
    public void AddHandler(Models.SubscriberMethod subscriberMethod)
    {
        if (subscriberMethod == null)
            throw new ArgumentNullException(nameof(subscriberMethod));
            
        var handlers = _handlers.GetOrAdd(subscriberMethod.EventType, _ => new ConcurrentBag<Models.SubscriberMethod>());
        handlers.Add(subscriberMethod);
    }
    
    /// <summary>
    /// Removes all handler methods associated with a specific subscriber instance.
    /// </summary>
    public void RemoveHandler(object subscriber)
    {
        if (subscriber == null)
            throw new ArgumentNullException(nameof(subscriber));
            
        foreach (var kvp in _handlers)
        {
            var handlers = kvp.Value;
            var toRemove = handlers.Where(h => ReferenceEquals(h.Subscriber, subscriber)).ToList();
            
            if (toRemove.Any())
            {
                // Create a new bag without the removed handlers
                var newBag = new ConcurrentBag<Models.SubscriberMethod>(
                    handlers.Except(toRemove)
                );
                _handlers.TryUpdate(kvp.Key, newBag, handlers);
            }
        }
    }
    
    /// <summary>
    /// Gets all handler methods for a specific event type, sorted by priority (descending).
    /// </summary>
    public IEnumerable<Models.SubscriberMethod> GetHandlers(Type eventType)
    {
        if (eventType == null)
            throw new ArgumentNullException(nameof(eventType));
            
        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            return handlers.OrderByDescending(h => h.Priority).ToList();
        }
        
        return Enumerable.Empty<Models.SubscriberMethod>();
    }
    
    /// <summary>
    /// Checks if there are any handlers registered for the specified event type.
    /// </summary>
    public bool HasHandlers(Type eventType)
    {
        if (eventType == null)
            throw new ArgumentNullException(nameof(eventType));
            
        return _handlers.TryGetValue(eventType, out var handlers) && handlers.Any();
    }
    
    /// <summary>
    /// Clears all registered handlers.
    /// </summary>
    public void Clear()
    {
        _handlers.Clear();
    }
}
