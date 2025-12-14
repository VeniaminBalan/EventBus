namespace EventBus.Core.Models;

/// <summary>
/// Event published when an event has no registered handlers.
/// </summary>
public class DeadEvent
{
    /// <summary>
    /// Gets the event that had no handlers.
    /// </summary>
    public object Event { get; }
    
    /// <summary>
    /// Gets the event bus that published the dead event.
    /// </summary>
    public object EventBus { get; }
    
    public DeadEvent(object eventObject, object eventBus)
    {
        Event = eventObject ?? throw new ArgumentNullException(nameof(eventObject));
        EventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }
}
