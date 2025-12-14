using EventBus.Samples.SensorMonitoring.Events;

namespace EventBus.Samples.SensorMonitoring.Sensors;

public class HumiditySensor : BaseSensor
{
    private readonly double _baseHumidity;

    public HumiditySensor(string sensorId, string region, Core.EventBus eventBus, double baseHumidity = 50.0)
        : base(sensorId, region, eventBus)
    {
        _baseHumidity = baseHumidity;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        IsRunning = true;
        Console.WriteLine($"Humidity Sensor {SensorId} started in {Region}");

        while (IsRunning && !cancellationToken.IsCancellationRequested)
        {
            // Simulate humidity reading (0-100%)
            double humidity = Math.Clamp(_baseHumidity + (Random.NextDouble() * 40 - 20), 0, 100);
            
            var readingEvent = new HumidityReadingEvent(humidity, SensorId, Region);
            await EventBus.PublishAsync(readingEvent);

            // Alert for very high humidity
            if (humidity > 85)
            {
                var alertEvent = new SensorAlertEvent(
                    "Humidity",
                    "WARNING",
                    humidity,
                    85,
                    Region
                );
                await EventBus.PublishAsync(alertEvent);
            }

            await Task.Delay(Random.Next(1500, 3500), cancellationToken);
        }
    }
}
