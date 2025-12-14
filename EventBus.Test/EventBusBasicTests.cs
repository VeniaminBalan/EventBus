using EventBus.Core;
using EventBus.Core.Attributes;
using Shouldly;

namespace EventBus.Test;

[TestClass]
public class EventBusBasicTests
{
    private Core.EventBus _eventBus = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _eventBus = new Core.EventBus();
    }
    
    #region Test Events
    
    public class SimpleEvent
    {
        public string Message { get; set; } = string.Empty;
    }
    
    public class AnotherEvent
    {
        public int Value { get; set; }
    }
    
    #endregion
    
    #region Test Subscribers
    
    public class SimpleSubscriber
    {
        public int CallCount { get; private set; }
        public SimpleEvent? LastEvent { get; private set; }
        
        [EventHandler]
        public void OnSimpleEvent(SimpleEvent evt)
        {
            CallCount++;
            LastEvent = evt;
        }
    }
    
    public class MultiEventSubscriber
    {
        public int SimpleEventCount { get; private set; }
        public int AnotherEventCount { get; private set; }
        
        [EventHandler]
        public void OnSimpleEvent(SimpleEvent evt)
        {
            SimpleEventCount++;
        }
        
        [EventHandler]
        public void OnAnotherEvent(AnotherEvent evt)
        {
            AnotherEventCount++;
        }
    }
    
    #endregion
    
    [TestMethod]
    public void Register_ShouldNotThrow_WhenValidSubscriber()
    {
        // Arrange
        var subscriber = new SimpleSubscriber();
        
        // Act & Assert
        Should.NotThrow(() => _eventBus.Register(subscriber));
    }
    
    [TestMethod]
    public void Register_ShouldThrow_WhenSubscriberIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _eventBus.Register(null!));
    }
    
    [TestMethod]
    public void Publish_ShouldInvokeHandler_WhenSubscriberRegistered()
    {
        // Arrange
        var subscriber = new SimpleSubscriber();
        _eventBus.Register(subscriber);
        var evt = new SimpleEvent { Message = "Test" };
        
        // Act
        _eventBus.Publish(evt);
        
        // Assert
        subscriber.CallCount.ShouldBe(1);
        subscriber.LastEvent.ShouldBe(evt);
        subscriber.LastEvent?.Message.ShouldBe("Test");
    }
    
    [TestMethod]
    public void Publish_ShouldInvokeMultipleTimes_WhenPublishedMultipleTimes()
    {
        // Arrange
        var subscriber = new SimpleSubscriber();
        _eventBus.Register(subscriber);
        
        // Act
        _eventBus.Publish(new SimpleEvent { Message = "First" });
        _eventBus.Publish(new SimpleEvent { Message = "Second" });
        _eventBus.Publish(new SimpleEvent { Message = "Third" });
        
        // Assert
        subscriber.CallCount.ShouldBe(3);
        subscriber.LastEvent?.Message.ShouldBe("Third");
    }
    
    [TestMethod]
    public void Publish_ShouldNotInvoke_AfterUnregister()
    {
        // Arrange
        var subscriber = new SimpleSubscriber();
        _eventBus.Register(subscriber);
        _eventBus.Publish(new SimpleEvent { Message = "Before" });
        
        // Act
        _eventBus.Unregister(subscriber);
        _eventBus.Publish(new SimpleEvent { Message = "After" });
        
        // Assert
        subscriber.CallCount.ShouldBe(1);
        subscriber.LastEvent?.Message.ShouldBe("Before");
    }
    
    [TestMethod]
    public void Publish_ShouldInvokeCorrectHandlers_ForDifferentEventTypes()
    {
        // Arrange
        var subscriber = new MultiEventSubscriber();
        _eventBus.Register(subscriber);
        
        // Act
        _eventBus.Publish(new SimpleEvent { Message = "Test" });
        _eventBus.Publish(new SimpleEvent { Message = "Test2" });
        _eventBus.Publish(new AnotherEvent { Value = 42 });
        
        // Assert
        subscriber.SimpleEventCount.ShouldBe(2);
        subscriber.AnotherEventCount.ShouldBe(1);
    }
    
    [TestMethod]
    public void Publish_ShouldInvokeAllSubscribers_WhenMultipleRegistered()
    {
        // Arrange
        var subscriber1 = new SimpleSubscriber();
        var subscriber2 = new SimpleSubscriber();
        var subscriber3 = new SimpleSubscriber();
        
        _eventBus.Register(subscriber1);
        _eventBus.Register(subscriber2);
        _eventBus.Register(subscriber3);
        
        var evt = new SimpleEvent { Message = "Broadcast" };
        
        // Act
        _eventBus.Publish(evt);
        
        // Assert
        subscriber1.CallCount.ShouldBe(1);
        subscriber2.CallCount.ShouldBe(1);
        subscriber3.CallCount.ShouldBe(1);
    }
    
    [TestMethod]
    public void Unregister_ShouldNotThrow_WhenSubscriberNotRegistered()
    {
        // Arrange
        var subscriber = new SimpleSubscriber();
        
        // Act & Assert
        Should.NotThrow(() => _eventBus.Unregister(subscriber));
    }
    
    [TestMethod]
    public void Publish_ShouldThrow_WhenEventIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _eventBus.Publish(null!));
    }
    
    [TestMethod]
    public void DefaultInstance_ShouldReturnSameInstance()
    {
        // Act
        var instance1 = Core.EventBus.Default;
        var instance2 = Core.EventBus.Default;
        
        // Assert
        instance1.ShouldBeSameAs(instance2);
    }
}
