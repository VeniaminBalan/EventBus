using EventBus.Core;
using EventBus.Core.Attributes;
using EventBus.Core.Exceptions;
using Shouldly;

namespace EventBus.Test;

[TestClass]
public class ExceptionHandlingTests
{
    public class TestEvent
    {
        public string Message { get; set; } = string.Empty;
    }
    
    public class ThrowingSubscriber
    {
        public bool WasCalled { get; private set; }
        
        [EventHandler]
        public void ThrowingHandler(TestEvent evt)
        {
            WasCalled = true;
            throw new InvalidOperationException("Test exception");
        }
    }
    
    public class ExceptionEventSubscriber
    {
        public SubscriberExceptionEvent? LastException { get; private set; }
        public int ExceptionCount { get; private set; }
        
        [EventHandler]
        public void OnException(SubscriberExceptionEvent evt)
        {
            LastException = evt;
            ExceptionCount++;
        }
    }
    
    public class MultipleHandlersSubscriber
    {
        public int FirstHandlerCalls { get; private set; }
        public int SecondHandlerCalls { get; private set; }
        
        [EventHandler(Priority = 10)]
        public void FirstHandler(TestEvent evt)
        {
            FirstHandlerCalls++;
            throw new Exception("First handler exception");
        }
        
        [EventHandler(Priority = 5)]
        public void SecondHandler(TestEvent evt)
        {
            SecondHandlerCalls++;
        }
    }
    
    [TestMethod]
    public void Publish_ShouldNotThrow_WhenHandlerThrowsAndThrowSubscriberExceptionIsFalse()
    {
        // Arrange
        var config = new EventBusConfiguration
        {
            ThrowSubscriberException = false,
            SendSubscriberExceptionEvent = false
        };
        var eventBus = new Core.EventBus(config);
        var subscriber = new ThrowingSubscriber();
        eventBus.Register(subscriber);
        
        // Act & Assert
        Should.NotThrow(() => eventBus.Publish(new TestEvent()));
        subscriber.WasCalled.ShouldBeTrue();
    }
    
    [TestMethod]
    public void Publish_ShouldThrow_WhenHandlerThrowsAndThrowSubscriberExceptionIsTrue()
    {
        // Arrange
        var config = new EventBusConfiguration
        {
            ThrowSubscriberException = true,
            SendSubscriberExceptionEvent = false
        };
        var eventBus = new Core.EventBus(config);
        var subscriber = new ThrowingSubscriber();
        eventBus.Register(subscriber);
        
        // Act & Assert
        Should.Throw<EventBusException>(() => eventBus.Publish(new TestEvent()));
    }
    
    [TestMethod]
    public void Publish_ShouldPublishExceptionEvent_WhenHandlerThrowsAndSendSubscriberExceptionEventIsTrue()
    {
        // Arrange
        var config = new EventBusConfiguration
        {
            ThrowSubscriberException = false,
            SendSubscriberExceptionEvent = true
        };
        var eventBus = new Core.EventBus(config);
        
        var throwingSubscriber = new ThrowingSubscriber();
        var exceptionSubscriber = new ExceptionEventSubscriber();
        
        eventBus.Register(throwingSubscriber);
        eventBus.Register(exceptionSubscriber);
        
        var testEvent = new TestEvent { Message = "Test" };
        
        // Act
        eventBus.Publish(testEvent);
        
        // Assert
        exceptionSubscriber.ExceptionCount.ShouldBe(1);
        exceptionSubscriber.LastException.ShouldNotBeNull();
        exceptionSubscriber.LastException!.Exception.ShouldBeOfType<InvalidOperationException>();
        exceptionSubscriber.LastException.Exception.Message.ShouldBe("Test exception");
        exceptionSubscriber.LastException.CausingEvent.ShouldBe(testEvent);
        exceptionSubscriber.LastException.CausingSubscriber.ShouldBe(throwingSubscriber);
    }
    
    [TestMethod]
    public void Publish_ShouldContinueExecuting_WhenOneHandlerThrows()
    {
        // Arrange
        var config = new EventBusConfiguration
        {
            ThrowSubscriberException = false,
            SendSubscriberExceptionEvent = false
        };
        var eventBus = new Core.EventBus(config);
        var subscriber = new MultipleHandlersSubscriber();
        eventBus.Register(subscriber);
        
        // Act
        eventBus.Publish(new TestEvent());
        
        // Assert
        subscriber.FirstHandlerCalls.ShouldBe(1);
        subscriber.SecondHandlerCalls.ShouldBe(1);
    }
}
