using EventBus.Core;
using EventBus.Core.Attributes;
using EventBus.Core.Exceptions;
using Shouldly;

namespace EventBus.Test;

[TestClass]
public class ValidationTests
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
    
    public class InvalidSubscriberNoParameters
    {
        [EventHandler]
        public void InvalidHandler()
        {
            // No parameters
        }
    }
    
    public class InvalidSubscriberTwoParameters
    {
        [EventHandler]
        public void InvalidHandler(TestEvent evt1, TestEvent evt2)
        {
            // Too many parameters
        }
    }
    
    public class InvalidSubscriberPrimitiveParameter
    {
        [EventHandler]
        public void InvalidHandler(int number)
        {
            // Primitive parameter
        }
    }
    
    public class ValidSubscriberPrivateMethod
    {
        public bool WasCalled { get; private set; }
        
        [EventHandler]
        private void PrivateHandler(TestEvent evt)
        {
            WasCalled = true;
        }
    }
    
    [TestMethod]
    public void Register_ShouldThrow_WhenHandlerHasNoParameters()
    {
        // Arrange
        var subscriber = new InvalidSubscriberNoParameters();
        
        // Act & Assert
        var exception = Should.Throw<EventBusException>(() => _eventBus.Register(subscriber));
        exception.Message.ShouldContain("exactly one parameter");
    }
    
    [TestMethod]
    public void Register_ShouldThrow_WhenHandlerHasTwoParameters()
    {
        // Arrange
        var subscriber = new InvalidSubscriberTwoParameters();
        
        // Act & Assert
        var exception = Should.Throw<EventBusException>(() => _eventBus.Register(subscriber));
        exception.Message.ShouldContain("exactly one parameter");
    }
    
    [TestMethod]
    public void Register_ShouldThrow_WhenHandlerParameterIsPrimitive()
    {
        // Arrange
        var subscriber = new InvalidSubscriberPrimitiveParameter();
        
        // Act & Assert
        var exception = Should.Throw<EventBusException>(() => _eventBus.Register(subscriber));
        exception.Message.ShouldContain("primitive type");
    }
    
    [TestMethod]
    public void Register_ShouldWork_WithPrivateHandlerMethod()
    {
        // Arrange
        var subscriber = new ValidSubscriberPrivateMethod();
        
        // Act
        _eventBus.Register(subscriber);
        _eventBus.Publish(new TestEvent { Message = "Test" });
        
        // Assert
        subscriber.WasCalled.ShouldBeTrue();
    }
}
