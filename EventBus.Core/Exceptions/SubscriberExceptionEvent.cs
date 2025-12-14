namespace EventBus.Core.Exceptions;

/// <summary>
/// Event published when a handler throws an exception during event processing.
/// </summary>
public class SubscriberExceptionEvent
{
    /// <summary>
    /// Gets the event bus that was processing the event.
    /// </summary>
    public object EventBus { get; }
    
    /// <summary>
    /// Gets the exception that was thrown.
    /// </summary>
    public Exception Exception { get; }
    
    /// <summary>
    /// Gets the event that was being processed when the exception occurred.
    /// </summary>
    public object CausingEvent { get; }
    
    /// <summary>
    /// Gets the subscriber instance that threw the exception.
    /// </summary>
    public object CausingSubscriber { get; }
    
    public SubscriberExceptionEvent(object eventBus, Exception exception, object causingEvent, object causingSubscriber)
    {
        EventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        CausingEvent = causingEvent ?? throw new ArgumentNullException(nameof(causingEvent));
        CausingSubscriber = causingSubscriber ?? throw new ArgumentNullException(nameof(causingSubscriber));
    }
}
