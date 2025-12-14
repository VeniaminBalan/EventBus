namespace EventBus.Core.Exceptions;

/// <summary>
/// Base exception for EventBus-specific errors.
/// </summary>
public class EventBusException : Exception
{
    public EventBusException(string message) : base(message)
    {
    }
    
    public EventBusException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
