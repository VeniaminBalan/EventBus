namespace EventBus.Samples.SensorMonitoring.Events;

public class TemperatureReadingEvent
{
    public double Temperature { get; set; }
    public string SensorId { get; set; }
    public string Region { get; set; }
    public DateTime Timestamp { get; set; }

    public TemperatureReadingEvent(double temperature, string sensorId, string region)
    {
        Temperature = temperature;
        SensorId = sensorId;
        Region = region;
        Timestamp = DateTime.Now;
    }

    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] Temperature: {Temperature:F1}°C | Sensor: {SensorId} | Region: {Region}";
    }
}
