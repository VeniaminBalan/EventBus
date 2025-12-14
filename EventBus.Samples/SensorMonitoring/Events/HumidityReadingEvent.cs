namespace EventBus.Samples.SensorMonitoring.Events;

public class HumidityReadingEvent
{
    public double Humidity { get; set; }
    public string SensorId { get; set; }
    public string Region { get; set; }
    public DateTime Timestamp { get; set; }

    public HumidityReadingEvent(double humidity, string sensorId, string region)
    {
        Humidity = humidity;
        SensorId = sensorId;
        Region = region;
        Timestamp = DateTime.Now;
    }

    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] Humidity: {Humidity:F1}% | Sensor: {SensorId} | Region: {Region}";
    }
}
