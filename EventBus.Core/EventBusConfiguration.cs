namespace EventBus.Core;

/// <summary>
/// Configuration options for EventBus behavior.
/// </summary>
public class EventBusConfiguration
{
    /// <summary>
    /// Gets or sets whether to throw exceptions from subscribers back to the publisher.
    /// Default is false.
    /// </summary>
    public bool ThrowSubscriberException { get; set; } = false;
    
    /// <summary>
    /// Gets or sets whether to send a SubscriberExceptionEvent when a handler throws.
    /// Default is true.
    /// </summary>
    public bool SendSubscriberExceptionEvent { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to log subscriber exceptions.
    /// Default is true.
    /// </summary>
    public bool LogSubscriberExceptions { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to log when no subscribers are found for an event.
    /// Default is false.
    /// </summary>
    public bool LogNoSubscriberMessages { get; set; } = false;
    
    /// <summary>
    /// Gets or sets whether to send a DeadEvent when no subscribers are found.
    /// Default is false.
    /// </summary>
    public bool SendNoSubscriberEvent { get; set; } = false;
    
    /// <summary>
    /// Gets or sets whether to use weak references for subscribers to prevent memory leaks.
    /// Default is false.
    /// </summary>
    public bool UseWeakReferences { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the maximum depth for event inheritance matching.
    /// Default is int.MaxValue (unlimited).
    /// </summary>
    public int EventInheritanceDepth { get; set; } = int.MaxValue;
}
