using EventBus.Core.Attributes;
using EventBus.Samples.SensorMonitoring.Events;

namespace EventBus.Samples.SensorMonitoring.Displays;

public class NumericDisplay
{
    private readonly string _displayId;
    private readonly string? _regionFilter;

    public NumericDisplay(string displayId, string? regionFilter = null)
    {
        _displayId = displayId;
        _regionFilter = regionFilter;
    }

    [EventHandler]
    public void OnTemperatureReading(TemperatureReadingEvent evt)
    {
        if (_regionFilter != null && evt.Region != _regionFilter) return;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[NumericDisplay-{_displayId}] {evt}");
        Console.ResetColor();
    }

    [EventHandler]
    public void OnHumidityReading(HumidityReadingEvent evt)
    {
        if (_regionFilter != null && evt.Region != _regionFilter) return;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[NumericDisplay-{_displayId}] {evt}");
        Console.ResetColor();
    }

    [EventHandler]
    public void OnWaterLevelReading(WaterLevelReadingEvent evt)
    {
        if (_regionFilter != null && evt.Location != _regionFilter) return;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"[NumericDisplay-{_displayId}] {evt}");
        Console.ResetColor();
    }
}
