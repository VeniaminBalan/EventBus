using EventBus.Core;
using EventBus.Core.Attributes;
using Shouldly;

namespace EventBus.Test;

[TestClass]
public class PriorityTests
{
    private Core.EventBus _eventBus = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _eventBus = new Core.EventBus();
    }
    
    public class TestEvent
    {
        public string Message { get; set; } = string.Empty;
    }
    
    public class PrioritySubscriber
    {
        public List<string> ExecutionOrder { get; } = new();
        
        [EventHandler(Priority = 10)]
        public void HighPriorityHandler(TestEvent evt)
        {
            ExecutionOrder.Add("High");
        }
        
        [EventHandler(Priority = 5)]
        public void MediumPriorityHandler(TestEvent evt)
        {
            ExecutionOrder.Add("Medium");
        }
        
        [EventHandler(Priority = 1)]
        public void LowPriorityHandler(TestEvent evt)
        {
            ExecutionOrder.Add("Low");
        }
        
        [EventHandler] // Default priority = 0
        public void DefaultPriorityHandler(TestEvent evt)
        {
            ExecutionOrder.Add("Default");
        }
    }
    
    [TestMethod]
    public void Publish_ShouldExecuteInPriorityOrder_HighToLow()
    {
        // Arrange
        var subscriber = new PrioritySubscriber();
        _eventBus.Register(subscriber);
        
        // Act
        _eventBus.Publish(new TestEvent { Message = "Test" });
        
        // Assert
        subscriber.ExecutionOrder.Count.ShouldBe(4);
        subscriber.ExecutionOrder[0].ShouldBe("High");
        subscriber.ExecutionOrder[1].ShouldBe("Medium");
        subscriber.ExecutionOrder[2].ShouldBe("Low");
        subscriber.ExecutionOrder[3].ShouldBe("Default");
    }
    
    [TestMethod]
    public void Publish_ShouldRespectPriority_AcrossMultipleSubscribers()
    {
        // Arrange
        var executionOrder = new List<string>();
        
        var subscriber1 = new SubscriberWithPriority5(executionOrder, "S1");
        var subscriber2 = new SubscriberWithPriority10(executionOrder, "S2");
        var subscriber3 = new SubscriberWithPriority1(executionOrder, "S3");
        
        _eventBus.Register(subscriber1);
        _eventBus.Register(subscriber2);
        _eventBus.Register(subscriber3);
        
        // Act
        _eventBus.Publish(new TestEvent());
        
        // Assert
        executionOrder.Count.ShouldBe(3);
        executionOrder[0].ShouldBe("S2"); // Priority 10
        executionOrder[1].ShouldBe("S1"); // Priority 5
        executionOrder[2].ShouldBe("S3"); // Priority 1
    }
    
    private class SubscriberWithPriority10
    {
        private readonly List<string> _executionOrder;
        private readonly string _name;
        
        public SubscriberWithPriority10(List<string> executionOrder, string name)
        {
            _executionOrder = executionOrder;
            _name = name;
        }
        
        [EventHandler(Priority = 10)]
        public void OnEvent(TestEvent evt)
        {
            _executionOrder.Add(_name);
        }
    }
    
    private class SubscriberWithPriority5
    {
        private readonly List<string> _executionOrder;
        private readonly string _name;
        
        public SubscriberWithPriority5(List<string> executionOrder, string name)
        {
            _executionOrder = executionOrder;
            _name = name;
        }
        
        [EventHandler(Priority = 5)]
        public void OnEvent(TestEvent evt)
        {
            _executionOrder.Add(_name);
        }
    }
    
    private class SubscriberWithPriority1
    {
        private readonly List<string> _executionOrder;
        private readonly string _name;
        
        public SubscriberWithPriority1(List<string> executionOrder, string name)
        {
            _executionOrder = executionOrder;
            _name = name;
        }
        
        [EventHandler(Priority = 1)]
        public void OnEvent(TestEvent evt)
        {
            _executionOrder.Add(_name);
        }
    }
}
