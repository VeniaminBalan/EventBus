using EventBus.Core.Attributes;
using EventBus.Samples.SensorMonitoring.Events;

namespace EventBus.Samples.SensorMonitoring.Displays;

public class DetailedDisplay
{
    private readonly string _displayId;
    private readonly Dictionary<string, List<string>> _sensorHistory = new();

    public DetailedDisplay(string displayId)
    {
        _displayId = displayId;
    }

    [EventHandler]
    public void OnTemperatureReading(TemperatureReadingEvent evt)
    {
        AddToHistory(evt.SensorId, $"Temperature: {evt.Temperature:F1}°C at {evt.Timestamp:HH:mm:ss}");
    }

    [EventHandler]
    public void OnWaterLevelReading(WaterLevelReadingEvent evt)
    {
        AddToHistory(evt.SensorId, $"Water Level: {evt.Level:F2}m at {evt.Timestamp:HH:mm:ss}");
    }

    private void AddToHistory(string sensorId, string reading)
    {
        if (!_sensorHistory.ContainsKey(sensorId))
        {
            _sensorHistory[sensorId] = new List<string>();
        }

        _sensorHistory[sensorId].Add(reading);
        
        // Keep only last 5 readings per sensor
        if (_sensorHistory[sensorId].Count > 5)
        {
            _sensorHistory[sensorId].RemoveAt(0);
        }

        // Display detailed view periodically
        if (_sensorHistory[sensorId].Count == 5)
        {
            DisplayHistory(sensorId);
        }
    }

    private void DisplayHistory(string sensorId)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n[DetailedDisplay-{_displayId}] History for {sensorId}:");
        foreach (var reading in _sensorHistory[sensorId])
        {
            Console.WriteLine($"  • {reading}");
        }
        Console.WriteLine();
        Console.ResetColor();
    }
}
