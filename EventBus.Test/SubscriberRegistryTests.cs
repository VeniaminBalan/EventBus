using EventBus.Core.Models;
using EventBus.Core.Registry;
using Shouldly;
using System.Reflection;

namespace EventBus.Test;

[TestClass]
public class SubscriberRegistryTests
{
    private SubscriberRegistry _registry = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _registry = new SubscriberRegistry();
    }
    
    public class TestEvent
    {
        public string Message { get; set; } = string.Empty;
    }
    
    public class AnotherEvent
    {
        public int Value { get; set; }
    }
    
    public class TestSubscriber
    {
        public void Handler(TestEvent evt) { }
        public void AnotherHandler(AnotherEvent evt) { }
    }
    
    [TestMethod]
    public void AddHandler_ShouldAddHandlerToRegistry()
    {
        // Arrange
        var subscriber = new TestSubscriber();
        var method = subscriber.GetType().GetMethod(nameof(TestSubscriber.Handler))!;
        var subscriberMethod = new SubscriberMethod(subscriber, method, typeof(TestEvent), 0, Core.Enums.ThreadMode.Posting);
        
        // Act
        _registry.AddHandler(subscriberMethod);
        
        // Assert
        _registry.HasHandlers(typeof(TestEvent)).ShouldBeTrue();
    }
    
    [TestMethod]
    public void AddHandler_ShouldThrow_WhenSubscriberMethodIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _registry.AddHandler(null!));
    }
    
    [TestMethod]
    public void GetHandlers_ShouldReturnHandlers_WhenHandlersExist()
    {
        // Arrange
        var subscriber = new TestSubscriber();
        var method = subscriber.GetType().GetMethod(nameof(TestSubscriber.Handler))!;
        var subscriberMethod = new SubscriberMethod(subscriber, method, typeof(TestEvent), 0, Core.Enums.ThreadMode.Posting);
        _registry.AddHandler(subscriberMethod);
        
        // Act
        var handlers = _registry.GetHandlers(typeof(TestEvent));
        
        // Assert
        handlers.ShouldNotBeEmpty();
        handlers.Count().ShouldBe(1);
        handlers.First().ShouldBe(subscriberMethod);
    }
    
    [TestMethod]
    public void GetHandlers_ShouldReturnEmpty_WhenNoHandlersExist()
    {
        // Act
        var handlers = _registry.GetHandlers(typeof(TestEvent));
        
        // Assert
        handlers.ShouldBeEmpty();
    }
    
    [TestMethod]
    public void GetHandlers_ShouldReturnHandlersSortedByPriority()
    {
        // Arrange
        var subscriber = new TestSubscriber();
        var method = subscriber.GetType().GetMethod(nameof(TestSubscriber.Handler))!;
        
        var lowPriority = new SubscriberMethod(subscriber, method, typeof(TestEvent), 1, Core.Enums.ThreadMode.Posting);
        var mediumPriority = new SubscriberMethod(subscriber, method, typeof(TestEvent), 5, Core.Enums.ThreadMode.Posting);
        var highPriority = new SubscriberMethod(subscriber, method, typeof(TestEvent), 10, Core.Enums.ThreadMode.Posting);
        
        _registry.AddHandler(lowPriority);
        _registry.AddHandler(highPriority);
        _registry.AddHandler(mediumPriority);
        
        // Act
        var handlers = _registry.GetHandlers(typeof(TestEvent)).ToList();
        
        // Assert
        handlers.Count.ShouldBe(3);
        handlers[0].Priority.ShouldBe(10);
        handlers[1].Priority.ShouldBe(5);
        handlers[2].Priority.ShouldBe(1);
    }
    
    [TestMethod]
    public void RemoveHandler_ShouldRemoveAllHandlersForSubscriber()
    {
        // Arrange
        var subscriber = new TestSubscriber();
        var method1 = subscriber.GetType().GetMethod(nameof(TestSubscriber.Handler))!;
        var method2 = subscriber.GetType().GetMethod(nameof(TestSubscriber.AnotherHandler))!;
        
        var handler1 = new SubscriberMethod(subscriber, method1, typeof(TestEvent), 0, Core.Enums.ThreadMode.Posting);
        var handler2 = new SubscriberMethod(subscriber, method2, typeof(AnotherEvent), 0, Core.Enums.ThreadMode.Posting);
        
        _registry.AddHandler(handler1);
        _registry.AddHandler(handler2);
        
        // Act
        _registry.RemoveHandler(subscriber);
        
        // Assert
        _registry.GetHandlers(typeof(TestEvent)).ShouldBeEmpty();
        _registry.GetHandlers(typeof(AnotherEvent)).ShouldBeEmpty();
    }
    
    [TestMethod]
    public void RemoveHandler_ShouldOnlyRemoveSpecificSubscriber()
    {
        // Arrange
        var subscriber1 = new TestSubscriber();
        var subscriber2 = new TestSubscriber();
        
        var method = typeof(TestSubscriber).GetMethod(nameof(TestSubscriber.Handler))!;
        
        var handler1 = new SubscriberMethod(subscriber1, method, typeof(TestEvent), 0, Core.Enums.ThreadMode.Posting);
        var handler2 = new SubscriberMethod(subscriber2, method, typeof(TestEvent), 0, Core.Enums.ThreadMode.Posting);
        
        _registry.AddHandler(handler1);
        _registry.AddHandler(handler2);
        
        // Act
        _registry.RemoveHandler(subscriber1);
        
        // Assert
        var handlers = _registry.GetHandlers(typeof(TestEvent)).ToList();
        handlers.Count.ShouldBe(1);
        handlers[0].Subscriber.ShouldBe(subscriber2);
    }
    
    [TestMethod]
    public void HasHandlers_ShouldReturnTrue_WhenHandlersExist()
    {
        // Arrange
        var subscriber = new TestSubscriber();
        var method = subscriber.GetType().GetMethod(nameof(TestSubscriber.Handler))!;
        var subscriberMethod = new SubscriberMethod(subscriber, method, typeof(TestEvent), 0, Core.Enums.ThreadMode.Posting);
        _registry.AddHandler(subscriberMethod);
        
        // Act & Assert
        _registry.HasHandlers(typeof(TestEvent)).ShouldBeTrue();
    }
    
    [TestMethod]
    public void HasHandlers_ShouldReturnFalse_WhenNoHandlersExist()
    {
        // Act & Assert
        _registry.HasHandlers(typeof(TestEvent)).ShouldBeFalse();
    }
    
    [TestMethod]
    public void Clear_ShouldRemoveAllHandlers()
    {
        // Arrange
        var subscriber = new TestSubscriber();
        var method1 = subscriber.GetType().GetMethod(nameof(TestSubscriber.Handler))!;
        var method2 = subscriber.GetType().GetMethod(nameof(TestSubscriber.AnotherHandler))!;
        
        _registry.AddHandler(new SubscriberMethod(subscriber, method1, typeof(TestEvent), 0, Core.Enums.ThreadMode.Posting));
        _registry.AddHandler(new SubscriberMethod(subscriber, method2, typeof(AnotherEvent), 0, Core.Enums.ThreadMode.Posting));
        
        // Act
        _registry.Clear();
        
        // Assert
        _registry.HasHandlers(typeof(TestEvent)).ShouldBeFalse();
        _registry.HasHandlers(typeof(AnotherEvent)).ShouldBeFalse();
    }
}
