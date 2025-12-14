using EventBus.Core;
using EventBus.Core.Attributes;
using EventBus.Core.Exceptions;
using Shouldly;

namespace EventBus.Test;

[TestClass]
public class EventBusConstructorTests
{
    #region Test Events and Subscribers
    
    public class TestEvent
    {
        public string Message { get; set; } = string.Empty;
    }
    
    public class TestSubscriber
    {
        public int CallCount { get; private set; }
        public TestEvent? LastEvent { get; private set; }
        
        [EventHandler]
        public void OnTestEvent(TestEvent evt)
        {
            CallCount++;
            LastEvent = evt;
        }
    }
    
    #endregion
    
    #region Constructor with Action Tests
    
    [TestMethod]
    public void Constructor_WithAction_ShouldCreateInstance()
    {
        // Act
        var eventBus = new Core.EventBus(config =>
        {
            config.ThrowSubscriberException = true;
        });
        
        // Assert
        eventBus.ShouldNotBeNull();
    }
    
    [TestMethod]
    public void Constructor_WithAction_ShouldApplyConfiguration()
    {
        // Arrange & Act
        var eventBus = new Core.EventBus(config =>
        {
            config.ThrowSubscriberException = true;
            config.SendSubscriberExceptionEvent = false;
            config.LogSubscriberExceptions = false;
        });
        
        var subscriber = new ThrowingSubscriber();
        eventBus.Register(subscriber);
        
        // Assert - Should throw because ThrowSubscriberException is true
        Should.Throw<EventBusException>(() => eventBus.Publish(new TestEvent()));
    }
    
    [TestMethod]
    public void Constructor_WithAction_ShouldApplyMultipleConfigurationOptions()
    {
        // Arrange & Act
        var eventBus = new Core.EventBus(config =>
        {
            config.ThrowSubscriberException = false;
            config.SendSubscriberExceptionEvent = true;
            config.LogSubscriberExceptions = true;
            config.LogNoSubscriberMessages = true;
            config.SendNoSubscriberEvent = true;
        });
        
        var subscriber = new TestSubscriber();
        eventBus.Register(subscriber);
        var testEvent = new TestEvent { Message = "Test" };
        
        // Act
        eventBus.Publish(testEvent);
        
        // Assert
        subscriber.CallCount.ShouldBe(1);
        subscriber.LastEvent.ShouldBe(testEvent);
    }
    
    [TestMethod]
    public void Constructor_WithAction_ShouldThrowArgumentNullException_WhenActionIsNull()
    {
        // Arrange
        Action<EventBusConfiguration>? nullAction = null;
        
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new Core.EventBus(nullAction!));
    }
    
    [TestMethod]
    public void Constructor_WithAction_ShouldUseDefaultValues_WhenNoPropertiesSet()
    {
        // Arrange & Act
        var eventBus = new Core.EventBus(config =>
        {
            // Don't set any properties, use defaults
        });
        
        var subscriber = new ThrowingSubscriber();
        eventBus.Register(subscriber);
        
        // Assert - Should not throw because default ThrowSubscriberException is false
        Should.NotThrow(() => eventBus.Publish(new TestEvent()));
    }
    
    [TestMethod]
    public void Constructor_WithAction_ShouldEnableLogNoSubscriberMessages()
    {
        // Arrange
        var eventBus = new Core.EventBus(config =>
        {
            config.LogNoSubscriberMessages = true;
        });
        
        // Act & Assert - Should not throw even when publishing to no subscribers
        Should.NotThrow(() => eventBus.Publish(new TestEvent()));
    }
    
    [TestMethod]
    public void Constructor_WithAction_ShouldEnableSendNoSubscriberEvent()
    {
        // Arrange
        var eventBus = new Core.EventBus(config =>
        {
            config.SendNoSubscriberEvent = true;
        });
        
        var deadEventSubscriber = new DeadEventSubscriber();
        eventBus.Register(deadEventSubscriber);
        
        var testEvent = new TestEvent { Message = "No subscribers" };
        
        // Act
        eventBus.Publish(testEvent);
        
        // Assert
        deadEventSubscriber.ReceivedDeadEvent.ShouldBeTrue();
        deadEventSubscriber.DeadEventContent.ShouldBe(testEvent);
    }
    
    [TestMethod]
    public void Constructor_WithAction_ShouldAllowFluentConfiguration()
    {
        // Arrange & Act
        var eventBus = new Core.EventBus(config =>
        {
            config.ThrowSubscriberException = false;
            config.SendSubscriberExceptionEvent = true;
            config.LogSubscriberExceptions = false;
            config.LogNoSubscriberMessages = false;
            config.SendNoSubscriberEvent = false;
            config.UseWeakReferences = false;
            config.EventInheritanceDepth = 5;
        });
        
        // Assert - EventBus should be created and functional
        eventBus.ShouldNotBeNull();
        var subscriber = new TestSubscriber();
        Should.NotThrow(() => eventBus.Register(subscriber));
    }
    
    [TestMethod]
    public void Constructor_WithAction_ShouldHandleExceptionProperly_WhenThrowIsEnabled()
    {
        // Arrange
        var eventBus = new Core.EventBus(config =>
        {
            config.ThrowSubscriberException = true;
            config.SendSubscriberExceptionEvent = false;
        });
        
        var subscriber = new ThrowingSubscriber();
        eventBus.Register(subscriber);
        
        // Act & Assert
        var exception = Should.Throw<EventBusException>(() => eventBus.Publish(new TestEvent()));
        exception.InnerException.ShouldNotBeNull();
        exception.InnerException!.Message.ShouldBe("Handler error");
    }
    
    [TestMethod]
    public void Constructor_WithAction_ShouldPublishSubscriberExceptionEvent_WhenConfigured()
    {
        // Arrange
        var exceptionEventSubscriber = new ExceptionEventSubscriber();
        
        var eventBus = new Core.EventBus(config =>
        {
            config.ThrowSubscriberException = false;
            config.SendSubscriberExceptionEvent = true;
        });
        
        eventBus.Register(exceptionEventSubscriber);
        var throwingSubscriber = new ThrowingSubscriber();
        eventBus.Register(throwingSubscriber);
        
        // Act
        eventBus.Publish(new TestEvent());
        
        // Assert
        exceptionEventSubscriber.ReceivedExceptionEvent.ShouldBeTrue();
        exceptionEventSubscriber.CaughtException.ShouldNotBeNull();
        exceptionEventSubscriber.CaughtException!.Message.ShouldBe("Handler error");
    }
    
    #endregion
    
    #region Constructor Comparison Tests
    
    [TestMethod]
    public void Constructor_WithAction_ShouldBehaveSameAs_ConfigurationObjectConstructor()
    {
        // Arrange
        var config = new EventBusConfiguration
        {
            ThrowSubscriberException = true,
            SendSubscriberExceptionEvent = false
        };
        
        var eventBusWithConfig = new Core.EventBus(config);
        var eventBusWithAction = new Core.EventBus(c =>
        {
            c.ThrowSubscriberException = true;
            c.SendSubscriberExceptionEvent = false;
        });
        
        var subscriber1 = new ThrowingSubscriber();
        var subscriber2 = new ThrowingSubscriber();
        
        eventBusWithConfig.Register(subscriber1);
        eventBusWithAction.Register(subscriber2);
        
        // Act & Assert - Both should throw
        Should.Throw<EventBusException>(() => eventBusWithConfig.Publish(new TestEvent()));
        Should.Throw<EventBusException>(() => eventBusWithAction.Publish(new TestEvent()));
    }
    
    [TestMethod]
    public void Constructor_Default_ShouldHaveDifferentBehavior_ThanCustomConfiguration()
    {
        // Arrange
        var defaultEventBus = new Core.EventBus();
        var customEventBus = new Core.EventBus(config =>
        {
            config.ThrowSubscriberException = true;
        });
        
        var subscriber1 = new ThrowingSubscriber();
        var subscriber2 = new ThrowingSubscriber();
        
        defaultEventBus.Register(subscriber1);
        customEventBus.Register(subscriber2);
        
        // Act & Assert
        Should.NotThrow(() => defaultEventBus.Publish(new TestEvent())); // Default doesn't throw
        Should.Throw<EventBusException>(() => customEventBus.Publish(new TestEvent())); // Custom throws
    }
    
    #endregion
    
    #region Test Helpers
    
    public class ThrowingSubscriber
    {
        public bool WasCalled { get; private set; }
        
        [EventHandler]
        public void OnTestEvent(TestEvent evt)
        {
            WasCalled = true;
            throw new InvalidOperationException("Handler error");
        }
    }
    
    public class DeadEventSubscriber
    {
        public bool ReceivedDeadEvent { get; private set; }
        public object? DeadEventContent { get; private set; }
        
        [EventHandler]
        public void OnDeadEvent(Core.Models.DeadEvent evt)
        {
            ReceivedDeadEvent = true;
            DeadEventContent = evt.Event;
        }
    }
    
    public class ExceptionEventSubscriber
    {
        public bool ReceivedExceptionEvent { get; private set; }
        public Exception? CaughtException { get; private set; }
        
        [EventHandler]
        public void OnExceptionEvent(SubscriberExceptionEvent evt)
        {
            ReceivedExceptionEvent = true;
            CaughtException = evt.Exception;
        }
    }
    
    #endregion
}

