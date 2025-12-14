using EventBus.Core;
using EventBus.Core.Attributes;
using EventBus.Core.Models;
using Shouldly;

namespace EventBus.Test;

[TestClass]
public class IntegrationTests
{
    [TestMethod]
    public void RealWorldScenario_UserLoginFlow()
    {
        // Arrange
        var eventBus = new Core.EventBus();
        var analyticsService = new AnalyticsService();
        var emailService = new EmailService();
        var loggingService = new LoggingService();
        
        eventBus.Register(analyticsService);
        eventBus.Register(emailService);
        eventBus.Register(loggingService);
        
        // Act
        eventBus.Publish(new UserLoggedInEvent
        {
            Username = "john.doe",
            LoginTime = DateTime.Now,
            IpAddress = "192.168.1.1"
        });
        
        // Assert
        analyticsService.LoginEvents.Count.ShouldBe(1);
        analyticsService.LoginEvents[0].Username.ShouldBe("john.doe");
        
        emailService.SentEmails.Count.ShouldBe(1);
        emailService.SentEmails[0].ShouldContain("john.doe");
        
        loggingService.Logs.Count.ShouldBe(1);
        loggingService.Logs[0].ShouldContain("User logged in");
    }
    
    [TestMethod]
    public void RealWorldScenario_OrderProcessingPipeline()
    {
        // Arrange
        var eventBus = new Core.EventBus();
        var orderValidator = new OrderValidationService();
        var inventoryService = new InventoryService();
        var paymentService = new PaymentService();
        var shippingService = new ShippingService();
        
        eventBus.Register(orderValidator);
        eventBus.Register(inventoryService);
        eventBus.Register(paymentService);
        eventBus.Register(shippingService);
        
        // Act
        eventBus.Publish(new OrderPlacedEvent
        {
            OrderId = 12345,
            TotalAmount = 99.99m,
            Items = new[] { "Item1", "Item2" }
        });
        
        // Assert
        orderValidator.ValidatedOrders.ShouldContain(12345);
        inventoryService.ReservedOrders.ShouldContain(12345);
        paymentService.ProcessedOrders.ShouldContain(12345);
        shippingService.ShippedOrders.ShouldContain(12345);
    }
    
    [TestMethod]
    public void RealWorldScenario_MultipleEventTypes()
    {
        // Arrange
        var eventBus = new Core.EventBus();
        var auditService = new AuditService();
        
        eventBus.Register(auditService);
        
        // Act
        eventBus.Publish(new UserLoggedInEvent { Username = "user1", LoginTime = DateTime.Now });
        eventBus.Publish(new OrderPlacedEvent { OrderId = 1, TotalAmount = 50m });
        eventBus.Publish(new UserLoggedOutEvent { Username = "user1", LogoutTime = DateTime.Now });
        eventBus.Publish(new OrderPlacedEvent { OrderId = 2, TotalAmount = 75m });
        
        // Assert
        auditService.TotalEvents.ShouldBe(4);
        auditService.LoginEvents.ShouldBe(1);
        auditService.LogoutEvents.ShouldBe(1);
        auditService.OrderEvents.ShouldBe(2);
    }
    
    [TestMethod]
    public void RealWorldScenario_EventChaining()
    {
        // Arrange
        var eventBus = new Core.EventBus();
        var orderService = new OrderService(eventBus);
        var notificationService = new NotificationService();
        
        eventBus.Register(orderService);
        eventBus.Register(notificationService);
        
        // Act - Publishing OrderPlaced should trigger OrderConfirmed
        eventBus.Publish(new OrderPlacedEvent { OrderId = 100, TotalAmount = 200m });
        
        // Assert
        orderService.ProcessedOrders.ShouldContain(100);
        notificationService.OrderPlacedNotifications.ShouldBe(1);
        notificationService.OrderConfirmedNotifications.ShouldBe(1);
    }
    
    #region Test Events
    
    public class UserLoggedInEvent
    {
        public string Username { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public string IpAddress { get; set; } = string.Empty;
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
        public string[] Items { get; set; } = Array.Empty<string>();
    }
    
    public class OrderConfirmedEvent
    {
        public int OrderId { get; set; }
        public DateTime ConfirmationTime { get; set; }
    }
    
    #endregion
    
    #region Test Services
    
    public class AnalyticsService
    {
        public List<UserLoggedInEvent> LoginEvents { get; } = new();
        
        [EventHandler]
        public void OnUserLogin(UserLoggedInEvent evt)
        {
            LoginEvents.Add(evt);
        }
    }
    
    public class EmailService
    {
        public List<string> SentEmails { get; } = new();
        
        [EventHandler]
        public void OnUserLogin(UserLoggedInEvent evt)
        {
            SentEmails.Add($"Welcome email sent to {evt.Username}");
        }
    }
    
    public class LoggingService
    {
        public List<string> Logs { get; } = new();
        
        [EventHandler]
        public void OnUserLogin(UserLoggedInEvent evt)
        {
            Logs.Add($"User logged in: {evt.Username} from {evt.IpAddress}");
        }
    }
    
    public class OrderValidationService
    {
        public HashSet<int> ValidatedOrders { get; } = new();
        
        [EventHandler(Priority = 100)]
        public void OnOrderPlaced(OrderPlacedEvent evt)
        {
            ValidatedOrders.Add(evt.OrderId);
        }
    }
    
    public class InventoryService
    {
        public HashSet<int> ReservedOrders { get; } = new();
        
        [EventHandler(Priority = 90)]
        public void OnOrderPlaced(OrderPlacedEvent evt)
        {
            ReservedOrders.Add(evt.OrderId);
        }
    }
    
    public class PaymentService
    {
        public HashSet<int> ProcessedOrders { get; } = new();
        
        [EventHandler(Priority = 80)]
        public void OnOrderPlaced(OrderPlacedEvent evt)
        {
            ProcessedOrders.Add(evt.OrderId);
        }
    }
    
    public class ShippingService
    {
        public HashSet<int> ShippedOrders { get; } = new();
        
        [EventHandler(Priority = 70)]
        public void OnOrderPlaced(OrderPlacedEvent evt)
        {
            ShippedOrders.Add(evt.OrderId);
        }
    }
    
    public class AuditService
    {
        public int TotalEvents { get; private set; }
        public int LoginEvents { get; private set; }
        public int LogoutEvents { get; private set; }
        public int OrderEvents { get; private set; }
        
        [EventHandler]
        public void OnUserLogin(UserLoggedInEvent evt)
        {
            TotalEvents++;
            LoginEvents++;
        }
        
        [EventHandler]
        public void OnUserLogout(UserLoggedOutEvent evt)
        {
            TotalEvents++;
            LogoutEvents++;
        }
        
        [EventHandler]
        public void OnOrderPlaced(OrderPlacedEvent evt)
        {
            TotalEvents++;
            OrderEvents++;
        }
    }
    
    public class OrderService
    {
        private readonly Core.EventBus _eventBus;
        public HashSet<int> ProcessedOrders { get; } = new();
        
        public OrderService(Core.EventBus eventBus)
        {
            _eventBus = eventBus;
        }
        
        [EventHandler]
        public void OnOrderPlaced(OrderPlacedEvent evt)
        {
            ProcessedOrders.Add(evt.OrderId);
            
            // Chain another event
            _eventBus.Publish(new OrderConfirmedEvent
            {
                OrderId = evt.OrderId,
                ConfirmationTime = DateTime.Now
            });
        }
    }
    
    public class NotificationService
    {
        public int OrderPlacedNotifications { get; private set; }
        public int OrderConfirmedNotifications { get; private set; }
        
        [EventHandler]
        public void OnOrderPlaced(OrderPlacedEvent evt)
        {
            OrderPlacedNotifications++;
        }
        
        [EventHandler]
        public void OnOrderConfirmed(OrderConfirmedEvent evt)
        {
            OrderConfirmedNotifications++;
        }
    }
    
    #endregion
}
