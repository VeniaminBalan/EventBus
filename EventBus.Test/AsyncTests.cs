using EventBus.Core;
using EventBus.Core.Attributes;
using EventBus.Core.Enums;
using Shouldly;

namespace EventBus.Test;

[TestClass]
public class AsyncTests
{
    private Core.EventBus _eventBus = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _eventBus = new Core.EventBus();
    }
    
    public class AsyncEvent
    {
        public string Message { get; set; } = string.Empty;
    }
    
    public class AsyncSubscriber
    {
        public int CallCount { get; private set; }
        public List<string> ExecutionLog { get; } = new();
        
        [EventHandler(ThreadMode = ThreadMode.Async)]
        public async Task OnAsyncEvent(AsyncEvent evt)
        {
            ExecutionLog.Add("Start");
            await Task.Delay(50);
            CallCount++;
            ExecutionLog.Add("End");
        }
    }
    
    public class MixedSubscriber
    {
        public int SyncCallCount { get; private set; }
        public int AsyncCallCount { get; private set; }
        
        [EventHandler(ThreadMode = ThreadMode.Posting)]
        public void OnSyncEvent(AsyncEvent evt)
        {
            SyncCallCount++;
        }
        
        [EventHandler(ThreadMode = ThreadMode.Async)]
        public async Task OnAsyncEvent(AsyncEvent evt)
        {
            await Task.Delay(10);
            AsyncCallCount++;
        }
    }
    
    [TestMethod]
    public async Task PublishAsync_ShouldInvokeAsyncHandler()
    {
        // Arrange
        var subscriber = new AsyncSubscriber();
        _eventBus.Register(subscriber);
        
        // Act
        await _eventBus.PublishAsync(new AsyncEvent { Message = "Test" });
        
        // Assert
        subscriber.CallCount.ShouldBe(1);
        subscriber.ExecutionLog.Count.ShouldBe(2);
        subscriber.ExecutionLog[0].ShouldBe("Start");
        subscriber.ExecutionLog[1].ShouldBe("End");
    }
    
    [TestMethod]
    public async Task PublishAsync_ShouldWaitForAllAsyncHandlers()
    {
        // Arrange
        var subscriber1 = new AsyncSubscriber();
        var subscriber2 = new AsyncSubscriber();
        _eventBus.Register(subscriber1);
        _eventBus.Register(subscriber2);
        
        // Act
        await _eventBus.PublishAsync(new AsyncEvent { Message = "Test" });
        
        // Assert
        subscriber1.CallCount.ShouldBe(1);
        subscriber2.CallCount.ShouldBe(1);
    }
    
    [TestMethod]
    public async Task PublishAsync_ShouldHandleMixedHandlers()
    {
        // Arrange
        var subscriber = new MixedSubscriber();
        _eventBus.Register(subscriber);
        
        // Act
        await _eventBus.PublishAsync(new AsyncEvent { Message = "Test" });
        
        // Assert
        subscriber.SyncCallCount.ShouldBe(1);
        subscriber.AsyncCallCount.ShouldBe(1);
    }
}
