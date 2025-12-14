using EventBus.Core.Enums;

namespace EventBus.Core.Attributes;

/// <summary>
/// Marks a method as an event handler that should receive events from the EventBus.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class EventHandlerAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the execution priority. Higher priority handlers execute first.
    /// </summary>
    public int Priority { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the threading mode for handler execution.
    /// </summary>
    public ThreadMode ThreadMode { get; set; } = ThreadMode.Posting;
}
