using EventBus.Core;
using EventBus.Core.Attributes;
using EventBus.Core.Models;
using Shouldly;

namespace EventBus.Test;

[TestClass]
public class DeadEventTests
{
    public class UnhandledEvent
    {
        public string Message { get; set; } = string.Empty;
    }
    
    public class DeadEventSubscriber
    {
        public DeadEvent? LastDeadEvent { get; private set; }
        public int DeadEventCount { get; private set; }
        
        [EventHandler]
        public void OnDeadEvent(DeadEvent evt)
        {
            LastDeadEvent = evt;
            DeadEventCount++;
        }
    }
    
    [TestMethod]
    public void Publish_ShouldNotPublishDeadEvent_WhenSendNoSubscriberEventIsFalse()
    {
        // Arrange
        var config = new EventBusConfiguration
        {
            SendNoSubscriberEvent = false
        };
        var eventBus = new Core.EventBus(config);
        var deadEventSubscriber = new DeadEventSubscriber();
        eventBus.Register(deadEventSubscriber);
        
        // Act
        eventBus.Publish(new UnhandledEvent { Message = "No handlers" });
        
        // Assert
        deadEventSubscriber.DeadEventCount.ShouldBe(0);
    }
    
    [TestMethod]
    public void Publish_ShouldPublishDeadEvent_WhenNoHandlersAndSendNoSubscriberEventIsTrue()
    {
        // Arrange
        var config = new EventBusConfiguration
        {
            SendNoSubscriberEvent = true
        };
        var eventBus = new Core.EventBus(config);
        var deadEventSubscriber = new DeadEventSubscriber();
        eventBus.Register(deadEventSubscriber);
        
        var unhandledEvent = new UnhandledEvent { Message = "No handlers" };
        
        // Act
        eventBus.Publish(unhandledEvent);
        
        // Assert
        deadEventSubscriber.DeadEventCount.ShouldBe(1);
        deadEventSubscriber.LastDeadEvent.ShouldNotBeNull();
        deadEventSubscriber.LastDeadEvent!.Event.ShouldBe(unhandledEvent);
        deadEventSubscriber.LastDeadEvent.EventBus.ShouldBe(eventBus);
    }
    
    [TestMethod]
    public void Publish_ShouldNotPublishDeadEvent_WhenHandlersExist()
    {
        // Arrange
        var config = new EventBusConfiguration
        {
            SendNoSubscriberEvent = true
        };
        var eventBus = new Core.EventBus(config);
        
        var deadEventSubscriber = new DeadEventSubscriber();
        var normalSubscriber = new NormalSubscriber();
        
        eventBus.Register(deadEventSubscriber);
        eventBus.Register(normalSubscriber);
        
        // Act
        eventBus.Publish(new UnhandledEvent { Message = "Has handlers" });
        
        // Assert
        deadEventSubscriber.DeadEventCount.ShouldBe(0);
        normalSubscriber.CallCount.ShouldBe(1);
    }
    
    private class NormalSubscriber
    {
        public int CallCount { get; private set; }
        
        [EventHandler]
        public void OnEvent(UnhandledEvent evt)
        {
            CallCount++;
        }
    }
}
