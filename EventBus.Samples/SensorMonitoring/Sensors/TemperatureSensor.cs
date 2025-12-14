using EventBus.Samples.SensorMonitoring.Events;

namespace EventBus.Samples.SensorMonitoring.Sensors;

public class TemperatureSensor : BaseSensor
{
    private readonly double _baseTemperature;
    private readonly double _criticalThreshold;

    public TemperatureSensor(string sensorId, string region, Core.EventBus eventBus, double baseTemperature = 20.0)
        : base(sensorId, region, eventBus)
    {
        _baseTemperature = baseTemperature;
        _criticalThreshold = baseTemperature + 15.0;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        IsRunning = true;
        Console.WriteLine($"Temperature Sensor {SensorId} started in {Region}");

        while (IsRunning && !cancellationToken.IsCancellationRequested)
        {
            // Simulate temperature reading with variation
            var temperature = _baseTemperature + (Random.NextDouble() * 20 - 5);
            
            var readingEvent = new TemperatureReadingEvent(temperature, SensorId, Region);
            await EventBus.PublishAsync(readingEvent);

            // Check for critical temperature
            if (temperature > _criticalThreshold)
            {
                var alertEvent = new SensorAlertEvent(
                    "Temperature",
                    "CRITICAL",
                    temperature,
                    _criticalThreshold,
                    Region
                );
                await EventBus.PublishAsync(alertEvent);
            }

            await Task.Delay(Random.Next(1000, 3000), cancellationToken);
        }
    }
}
