namespace EventBus.Core.Enums;

/// <summary>
/// Defines the threading behavior for event handler execution.
/// </summary>
public enum ThreadMode
{
    /// <summary>
    /// Handler executes on the publishing thread (default).
    /// </summary>
    Posting,
    
    /// <summary>
    /// Handler executes on a background thread.
    /// </summary>
    Background,
    
    /// <summary>
    /// Handler executes asynchronously.
    /// </summary>
    Async,
    
    /// <summary>
    /// Handler executes on the main/UI thread (if applicable).
    /// </summary>
    MainThread
}
