using EventBus.Core.Attributes;
using EventBus.Core.Enums;

namespace EventBus.Core;

/// <summary>
/// Sample application demonstrating EventBus usage
/// </summary>
public class SampleUsage
{
    public static void Main()
    {
        Console.WriteLine("=== EventBus Sample Usage ===\n");
        
        // Example 1: Basic Usage
        BasicUsageExample();
        
        // Example 2: Priority Handling
        PriorityExample();
        
        // Example 3: Async Events
        AsyncExample().Wait();
        
        // Example 4: Exception Handling
        ExceptionHandlingExample();
        
        // Example 5: Multiple Events
        MultipleEventsExample();
    }
    
    private static void BasicUsageExample()
    {
        Console.WriteLine("--- Example 1: Basic Usage ---");
        
        // Create an event bus instance
        var eventBus = new Core.EventBus();
        
        // Create and register a subscriber
        var userService = new UserService();
        eventBus.Register(userService);
        
        // Publish an event
        eventBus.Publish(new UserLoggedInEvent
        {
            Username = "john.doe",
            LoginTime = DateTime.Now
        });
        
        // Unregister when done
        eventBus.Unregister(userService);
        
        Console.WriteLine();
    }
    
    private static void PriorityExample()
    {
        Console.WriteLine("--- Example 2: Priority Handling ---");
        
        var eventBus = new Core.EventBus();
        var orderProcessor = new OrderProcessor();
        eventBus.Register(orderProcessor);
        
        eventBus.Publish(new OrderPlacedEvent
        {
            OrderId = 12345,
            TotalAmount = 99.99m
        });
        
        Console.WriteLine();
    }
    
    private static async Task AsyncExample()
    {
        Console.WriteLine("--- Example 3: Async Events ---");
        
        var eventBus = new Core.EventBus();
        var emailService = new EmailService();
        eventBus.Register(emailService);
        
        await eventBus.PublishAsync(new UserLoggedInEvent
        {
            Username = "jane.doe",
            LoginTime = DateTime.Now
        });
        
        Console.WriteLine();
    }
    
    private static void ExceptionHandlingExample()
    {
        Console.WriteLine("--- Example 4: Exception Handling ---");
        
        var config = new EventBusConfiguration
        {
            ThrowSubscriberException = false,
            SendSubscriberExceptionEvent = true
        };
        
        var eventBus = new Core.EventBus(config);
        var errorHandler = new ErrorHandler();
        var faultyService = new FaultyService();
        
        eventBus.Register(errorHandler);
        eventBus.Register(faultyService);
        
        eventBus.Publish(new TestEvent { Message = "This will cause an error" });
        
        Console.WriteLine();
    }
    
    private static void MultipleEventsExample()
    {
        Console.WriteLine("--- Example 5: Multiple Events ---");
        
        // Using the default singleton instance
        var analytics = new AnalyticsService();
        Core.EventBus.Default.Register(analytics);
        
        Core.EventBus.Default.Publish(new UserLoggedInEvent { Username = "user1", LoginTime = DateTime.Now });
        Core.EventBus.Default.Publish(new UserLoggedOutEvent { Username = "user1", LogoutTime = DateTime.Now });
        Core.EventBus.Default.Publish(new OrderPlacedEvent { OrderId = 1, TotalAmount = 50m });
        
        Console.WriteLine($"Total events processed: {analytics.EventCount}");
        
        Console.WriteLine();
    }
}

#region Sample Events

public class UserLoggedInEvent
{
    public string Username { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
}

public class UserLoggedOutEvent
{
    public string Username { get; set; } = string.Empty;
    public DateTime LogoutTime { get; set; }
}

public class OrderPlacedEvent
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
}

public class TestEvent
{
    public string Message { get; set; } = string.Empty;
}

#endregion

#region Sample Subscribers

public class UserService
{
    [EventHandler]
    public void OnUserLoggedIn(UserLoggedInEvent evt)
    {
        Console.WriteLine($"[UserService] User {evt.Username} logged in at {evt.LoginTime}");
    }
}

public class OrderProcessor
{
    [EventHandler(Priority = 100)]
    public void ValidateOrder(OrderPlacedEvent evt)
    {
        Console.WriteLine($"[OrderProcessor] Step 1: Validating order {evt.OrderId}");
    }
    
    [EventHandler(Priority = 90)]
    public void ReserveInventory(OrderPlacedEvent evt)
    {
        Console.WriteLine($"[OrderProcessor] Step 2: Reserving inventory for order {evt.OrderId}");
    }
    
    [EventHandler(Priority = 80)]
    public void ProcessPayment(OrderPlacedEvent evt)
    {
        Console.WriteLine($"[OrderProcessor] Step 3: Processing payment of ${evt.TotalAmount}");
    }
    
    [EventHandler(Priority = 70)]
    public void ShipOrder(OrderPlacedEvent evt)
    {
        Console.WriteLine($"[OrderProcessor] Step 4: Shipping order {evt.OrderId}");
    }
}

public class EmailService
{
    [EventHandler(ThreadMode = ThreadMode.Async)]
    public async Task SendWelcomeEmail(UserLoggedInEvent evt)
    {
        Console.WriteLine($"[EmailService] Preparing welcome email for {evt.Username}...");
        await Task.Delay(100); // Simulate email sending
        Console.WriteLine($"[EmailService] Welcome email sent to {evt.Username}");
    }
}

public class ErrorHandler
{
    [EventHandler]
    public void OnSubscriberException(global::EventBus.Core.Exceptions.SubscriberExceptionEvent evt)
    {
        Console.WriteLine($"[ErrorHandler] Exception caught: {evt.Exception.Message}");
        Console.WriteLine($"[ErrorHandler] Caused by event: {evt.CausingEvent.GetType().Name}");
    }
}

public class FaultyService
{
    [EventHandler]
    public void OnTestEvent(TestEvent evt)
    {
        Console.WriteLine($"[FaultyService] Processing: {evt.Message}");
        throw new InvalidOperationException("Simulated error in FaultyService");
    }
}

public class AnalyticsService
{
    public int EventCount { get; private set; }
    
    [EventHandler]
    public void OnUserLoggedIn(UserLoggedInEvent evt)
    {
        EventCount++;
        Console.WriteLine($"[Analytics] User login recorded: {evt.Username}");
    }
    
    [EventHandler]
    public void OnUserLoggedOut(UserLoggedOutEvent evt)
    {
        EventCount++;
        Console.WriteLine($"[Analytics] User logout recorded: {evt.Username}");
    }
    
    [EventHandler]
    public void OnOrderPlaced(OrderPlacedEvent evt)
    {
        EventCount++;
        Console.WriteLine($"[Analytics] Order recorded: {evt.OrderId} - ${evt.TotalAmount}");
    }
}

#endregion
