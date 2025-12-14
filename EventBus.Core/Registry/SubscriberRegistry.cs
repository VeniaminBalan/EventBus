using System.Collections.Concurrent;
using EventBus.Core.Models;

namespace EventBus.Core.Registry;

/// <summary>
/// Thread-safe registry that maps event types to their subscriber methods.
/// </summary>
public class SubscriberRegistry
{
    private readonly ConcurrentDictionary<Type, ConcurrentBag<SubscriberMethod>> _handlers = new();

    /// <summary>
    /// Adds a handler method for a specific event type.
    /// </summary>
    public void AddHandler(SubscriberMethod subscriberMethod)
    {
        ArgumentNullException.ThrowIfNull(subscriberMethod);

        var handlers = _handlers.GetOrAdd(subscriberMethod.EventType, _ => []);
        handlers.Add(subscriberMethod);
    }
    
    /// <summary>
    /// Removes all handler methods associated with a specific subscriber instance.
    /// </summary>
    public void RemoveHandler(object subscriber)
    {
        ArgumentNullException.ThrowIfNull(subscriber);

        foreach (var (type, handlers) in _handlers)
        {
            var toRemove = handlers.Where(h => ReferenceEquals(h.Subscriber, subscriber)).ToList();

            if (toRemove.Count == 0) continue;
            // Create a new bag without the removed handlers
            var newBag = new ConcurrentBag<SubscriberMethod>(
                handlers.Except(toRemove)
            );
            _handlers.TryUpdate(type, newBag, handlers);
        }
    }
    
    /// <summary>
    /// Gets all handler methods for a specific event type, sorted by priority (descending).
    /// </summary>
    public IEnumerable<SubscriberMethod> GetHandlers(Type eventType)
    {
        ArgumentNullException.ThrowIfNull(eventType);

        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            return handlers.OrderByDescending(h => h.Priority).ToList();
        }
        
        return [];
    }
    
    /// <summary>
    /// Checks if there are any handlers registered for the specified event type.
    /// </summary>
    public bool HasHandlers(Type eventType)
    {
        ArgumentNullException.ThrowIfNull(eventType);

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
