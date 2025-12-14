using EventBus.Core.Attributes;
using EventBus.Core.Enums;
using EventBus.Core.Models;
using Shouldly;
using System.Reflection;

namespace EventBus.Test;

[TestClass]
public class SubscriberMethodTests
{
    public class TestEvent
    {
        public string Message { get; set; } = string.Empty;
    }
    
    public class TestSubscriber
    {
        public TestEvent? LastEvent { get; private set; }
        public int CallCount { get; private set; }
        
        public void Handler(TestEvent evt)
        {
            LastEvent = evt;
            CallCount++;
        }
        
        public async Task AsyncHandler(TestEvent evt)
        {
            await Task.Delay(10);
            LastEvent = evt;
            CallCount++;
        }
    }
    
    [TestMethod]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var subscriber = new TestSubscriber();
        var method = subscriber.GetType().GetMethod(nameof(TestSubscriber.Handler))!;
        var eventType = typeof(TestEvent);
        
        // Act
        var subscriberMethod = new SubscriberMethod(subscriber, method, eventType, 5, ThreadMode.Posting);
        
        // Assert
        subscriberMethod.Subscriber.ShouldBe(subscriber);
        subscriberMethod.Method.ShouldBe(method);
        subscriberMethod.EventType.ShouldBe(eventType);
        subscriberMethod.Priority.ShouldBe(5);
        subscriberMethod.ThreadMode.ShouldBe(ThreadMode.Posting);
    }
    
    [TestMethod]
    public void Constructor_ShouldThrow_WhenSubscriberIsNull()
    {
        // Arrange
        var method = typeof(TestSubscriber).GetMethod(nameof(TestSubscriber.Handler))!;
        
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new SubscriberMethod(null!, method, typeof(TestEvent), 0, ThreadMode.Posting));
    }
    
    [TestMethod]
    public void Constructor_ShouldThrow_WhenMethodIsNull()
    {
        // Arrange
        var subscriber = new TestSubscriber();
        
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new SubscriberMethod(subscriber, null!, typeof(TestEvent), 0, ThreadMode.Posting));
    }
    
    [TestMethod]
    public void Invoke_ShouldCallMethod()
    {
        // Arrange
        var subscriber = new TestSubscriber();
        var method = subscriber.GetType().GetMethod(nameof(TestSubscriber.Handler))!;
        var subscriberMethod = new SubscriberMethod(subscriber, method, typeof(TestEvent), 0, ThreadMode.Posting);
        var evt = new TestEvent { Message = "Test" };
        
        // Act
        subscriberMethod.Invoke(evt);
        
        // Assert
        subscriber.CallCount.ShouldBe(1);
        subscriber.LastEvent.ShouldBe(evt);
    }
    
    [TestMethod]
    public void Invoke_ShouldThrow_WhenEventIsNull()
    {
        // Arrange
        var subscriber = new TestSubscriber();
        var method = subscriber.GetType().GetMethod(nameof(TestSubscriber.Handler))!;
        var subscriberMethod = new SubscriberMethod(subscriber, method, typeof(TestEvent), 0, ThreadMode.Posting);
        
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => subscriberMethod.Invoke(null!));
    }
    
    [TestMethod]
    public async Task InvokeAsync_ShouldCallAsyncMethod()
    {
        // Arrange
        var subscriber = new TestSubscriber();
        var method = subscriber.GetType().GetMethod(nameof(TestSubscriber.AsyncHandler))!;
        var subscriberMethod = new SubscriberMethod(subscriber, method, typeof(TestEvent), 0, ThreadMode.Async);
        var evt = new TestEvent { Message = "Async Test" };
        
        // Act
        await subscriberMethod.InvokeAsync(evt);
        
        // Assert
        subscriber.CallCount.ShouldBe(1);
        subscriber.LastEvent.ShouldBe(evt);
        subscriber.LastEvent?.Message.ShouldBe("Async Test");
    }
}
