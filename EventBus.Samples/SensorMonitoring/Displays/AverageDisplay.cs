using EventBus.Core.Attributes;
using EventBus.Samples.SensorMonitoring.Events;

namespace EventBus.Samples.SensorMonitoring.Displays;

public class AverageDisplay
{
    private readonly string _displayId;
    private readonly List<double> _temperatureReadings = new();
    private readonly List<double> _humidityReadings = new();
    private int _updateCounter = 0;

    public AverageDisplay(string displayId)
    {
        _displayId = displayId;
    }

    [EventHandler]
    public void OnTemperatureReading(TemperatureReadingEvent evt)
    {
        _temperatureReadings.Add(evt.Temperature);
        if (_temperatureReadings.Count > 10)
            _temperatureReadings.RemoveAt(0);

        _updateCounter++;
        if (_updateCounter % 5 == 0)
        {
            DisplayAverages();
        }
    }

    [EventHandler]
    public void OnHumidityReading(HumidityReadingEvent evt)
    {
        _humidityReadings.Add(evt.Humidity);
        if (_humidityReadings.Count > 10)
            _humidityReadings.RemoveAt(0);
    }

    private void DisplayAverages()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"\n[AverageDisplay-{_displayId}] --------------------------");
        
        if (_temperatureReadings.Count != 0)
        {
            var avgTemp = _temperatureReadings.Average();
            Console.WriteLine($"  Average Temperature: {avgTemp:F2}°C (based on {_temperatureReadings.Count} readings)");
        }
        
        if (_humidityReadings.Count != 0)
        {
            var avgHumidity = _humidityReadings.Average();
            Console.WriteLine($"  Average Humidity: {avgHumidity:F2}% (based on {_humidityReadings.Count} readings)");
        }
        
        Console.WriteLine($"--------------------------\n");
        Console.ResetColor();
    }
}
