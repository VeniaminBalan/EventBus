using EventBus.Samples.SensorMonitoring.Events;

namespace EventBus.Samples.SensorMonitoring.Sensors;

public abstract class BaseSensor
{
    protected string SensorId { get; set; }
    protected string Region { get; set; }
    protected Core.EventBus EventBus { get; set; }
    protected Random Random { get; set; }
    protected bool IsRunning { get; set; }

    protected BaseSensor(string sensorId, string region, Core.EventBus eventBus)
    {
        SensorId = sensorId;
        Region = region;
        EventBus = eventBus;
        Random = new Random();
        IsRunning = false;
    }

    public abstract Task StartAsync(CancellationToken cancellationToken);
    
    public void Stop()
    {
        IsRunning = false;
    }
}
