namespace EventBus.Samples.SensorMonitoring.Events;

public class WaterLevelReadingEvent
{
    public double Level { get; set; }
    public string SensorId { get; set; }
    public string Location { get; set; }
    public DateTime Timestamp { get; set; }

    public WaterLevelReadingEvent(double level, string sensorId, string location)
    {
        Level = level;
        SensorId = sensorId;
        Location = location;
        Timestamp = DateTime.Now;
    }

    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] Water Level: {Level:F2}m | Sensor: {SensorId} | Location: {Location}";
    }
}
