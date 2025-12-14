namespace EventBus.Core.Models;

/// <summary>
/// Optional base class for events. Events can be any type; this is provided for convenience.
/// </summary>
public abstract class Event
{
    /// <summary>
    /// Gets the timestamp when the event was created.
    /// </summary>
    public DateTime Timestamp { get; }
    
    /// <summary>
    /// Gets the unique identifier for this event instance.
    /// </summary>
    public Guid EventId { get; }
    
    protected Event()
    {
        Timestamp = DateTime.UtcNow;
        EventId = Guid.NewGuid();
    }
}
