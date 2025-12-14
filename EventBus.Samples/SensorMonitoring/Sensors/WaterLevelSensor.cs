using EventBus.Samples.SensorMonitoring.Events;

namespace EventBus.Samples.SensorMonitoring.Sensors;

public class WaterLevelSensor : BaseSensor
{
    private readonly double _normalLevel;
    private readonly double _floodThreshold;

    public WaterLevelSensor(string sensorId, string location, Core.EventBus eventBus, double normalLevel = 2.5)
        : base(sensorId, location, eventBus)
    {
        _normalLevel = normalLevel;
        _floodThreshold = normalLevel + 1.5;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        IsRunning = true;
        Console.WriteLine($"🌊 Water Level Sensor {SensorId} started at {Region}");

        while (IsRunning && !cancellationToken.IsCancellationRequested)
        {
            // Simulate water level reading
            var level = Math.Max(0, _normalLevel + (Random.NextDouble() * 2.5 - 0.5));
            
            var readingEvent = new WaterLevelReadingEvent(level, SensorId, Region);
            await EventBus.PublishAsync(readingEvent);

            // Alert for flood risk
            if (level > _floodThreshold)
            {
                var alertEvent = new SensorAlertEvent(
                    "WaterLevel",
                    "CRITICAL",
                    level,
                    _floodThreshold,
                    Region
                );
                await EventBus.PublishAsync(alertEvent);
            }

            await Task.Delay(Random.Next(2000, 4000), cancellationToken);
        }
    }
}
