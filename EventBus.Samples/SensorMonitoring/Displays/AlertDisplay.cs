using EventBus.Core.Attributes;
using EventBus.Samples.SensorMonitoring.Events;

namespace EventBus.Samples.SensorMonitoring.Displays;

public class AlertDisplay
{
    private readonly string _displayId;

    public AlertDisplay(string displayId)
    {
        _displayId = displayId;
    }

    [EventHandler(Priority = 100)]  // High priority for alerts
    public void OnSensorAlert(SensorAlertEvent evt)
    {
        Console.ForegroundColor = evt.Severity == "CRITICAL" ? ConsoleColor.Red : ConsoleColor.DarkYellow;
        Console.WriteLine("\n-------------------------------------------------");
        Console.WriteLine($"| [AlertDisplay-{_displayId}] {evt}");
        Console.WriteLine("-------------------------------------------------\n");
        Console.ResetColor();
    }
}
