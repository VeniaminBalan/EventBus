namespace EventBus.Samples.SensorMonitoring.Events;

public class SensorAlertEvent
{
    public string SensorType { get; set; }
    public string Severity { get; set; }
    public double Value { get; set; }
    public double Threshold { get; set; }
    public string Region { get; set; }
    public DateTime Timestamp { get; set; }

    public SensorAlertEvent(string sensorType, string severity, double value, double threshold, string region)
    {
        SensorType = sensorType;
        Severity = severity;
        Value = value;
        Threshold = threshold;
        Region = region;
        Timestamp = DateTime.Now;
    }

    public override string ToString()
    {
        return $"!!! ALERT [{Timestamp:HH:mm:ss}] {Severity} - {SensorType}: {Value:F1} exceeds threshold {Threshold:F1} in {Region}";
    }
}
