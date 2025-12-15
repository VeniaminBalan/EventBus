using EventBus.Samples.SensorMonitoring.Sensors;
using EventBus.Samples.SensorMonitoring.Displays;
using EventBus.Samples.NewsAgency.Agencies;
using EventBus.Samples.NewsAgency.Subscribers;
using EventBus.Samples.NewsAgency.Events;

Console.WriteLine("Select a sample to run:");
Console.WriteLine("1. Sensor Monitoring System");
Console.WriteLine("2. News Agency System");
Console.WriteLine("3. Run Both Samples (Side by Side)");
Console.WriteLine("0. Exit");
Console.Write("\nYour choice: ");

var choice = Console.ReadLine();

switch (choice)
{
    case "1":
        await RunSensorMonitoringSample();
        break;
    case "2":
        await RunNewsAgencySample();
        break;
    case "3":
        await RunBothSamples();
        break;
    case "0":
        Console.WriteLine("Goodbye!");
        return;
    default:
        Console.WriteLine("Invalid choice. Exiting...");
        return;
}

return;

static async Task RunSensorMonitoringSample()
{
    Console.Clear();
    var eventBus = new EventBus.Core.EventBus();
    var cts = new CancellationTokenSource();

    // Create and register displays
    var numericDisplay1 = new NumericDisplay("North", "North");
    var numericDisplay2 = new NumericDisplay("South", "South");
    var averageDisplay = new AverageDisplay("Main");
    var detailedDisplay = new DetailedDisplay("Central");
    var alertDisplay = new AlertDisplay("Emergency");

    eventBus.Register(numericDisplay1);
    eventBus.Register(numericDisplay2);
    eventBus.Register(averageDisplay);
    eventBus.Register(detailedDisplay);
    eventBus.Register(alertDisplay);

    Console.WriteLine("Displays registered\n");

    // Create sensors
    var tempSensor1 = new TemperatureSensor("TEMP-001", "North", eventBus, 18.0);
    var tempSensor2 = new TemperatureSensor("TEMP-002", "South", eventBus, 22.0);
    var humiditySensor = new HumiditySensor("HUM-001", "North", eventBus, 60.0);
    var waterSensor = new WaterLevelSensor("WATER-001", "River Delta", eventBus, 2.0);

    Console.WriteLine("Starting sensors... (Press any key to stop)\n");

    // Start all sensors
    var tasks = new[]
    {
        tempSensor1.StartAsync(cts.Token),
        tempSensor2.StartAsync(cts.Token),
        humiditySensor.StartAsync(cts.Token),
        waterSensor.StartAsync(cts.Token)
    };

    // Wait for user input to stop
    await Task.Run(() => Console.ReadKey(true));
    
    Console.WriteLine("\n Stopping sensors...");
    cts.Cancel();

    try
    {
        await Task.WhenAll(tasks);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("All sensors stopped successfully");
    }

    Console.WriteLine("\nPress any key to return to exit...");
    Console.ReadKey();
}

static async Task RunNewsAgencySample()
{
    Console.Clear();
    var eventBus = new EventBus.Core.EventBus();
    var cts = new CancellationTokenSource();

    // Create and register subscribers
    var alice = new Person("Alice", NewsCategory.Technology, NewsCategory.Sports);
    var bob = new Person("Bob", NewsCategory.Politics, NewsCategory.Culture);
    var aggregator = new NewsAggregator("MainAggregator");
    var archive = new NewsArchive();
    var techSpecialist = new SpecializedReader("Dr. Smith", NewsCategory.Technology);
    var sportsSpecialist = new SpecializedReader("Coach Johnson", NewsCategory.Sports);

    eventBus.Register(alice);
    eventBus.Register(bob);
    eventBus.Register(aggregator);
    eventBus.Register(archive);
    eventBus.Register(techSpecialist);
    eventBus.Register(sportsSpecialist);

    Console.WriteLine("Subscribers registered\n");

    // Create agencies
    var sportsAgency = new SportsNewsAgency(eventBus);
    var politicsAgency = new PoliticsNewsAgency(eventBus);
    var techAgency = new TechnologyNewsAgency(eventBus);
    var cultureAgency = new CultureNewsAgency(eventBus);

    Console.WriteLine("Starting news agencies... (Press any key to stop)\n");

    // Start all agencies
    var tasks = new[]
    {
        sportsAgency.StartPublishingAsync(cts.Token),
        politicsAgency.StartPublishingAsync(cts.Token),
        techAgency.StartPublishingAsync(cts.Token),
        cultureAgency.StartPublishingAsync(cts.Token)
    };

    // Simulate dynamic subscription changes
    _ = Task.Run(async () =>
    {
        await Task.Delay(10000);
        if (!cts.Token.IsCancellationRequested)
        {
            Console.WriteLine("\n--- Dynamic Subscription Change ---");
            alice.AddInterest(NewsCategory.Politics);
            bob.RemoveInterest(NewsCategory.Culture);
            Console.WriteLine("-----------------------------------\n");
        }
    });

    // Wait for user input to stop
    await Task.Run(() => Console.ReadKey(true));
    
    Console.WriteLine("\n\nStopping news agencies...");
    cts.Cancel();

    try
    {
        await Task.WhenAll(tasks);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("All agencies stopped successfully");
    }

    archive.DisplayArchiveSummary();

    Console.WriteLine("\nPress any key to return to menu...");
    Console.ReadKey();
}

static async Task RunBothSamples()
{
    Console.Clear();

    // Use separate event buses for each system
    var sensorEventBus = new EventBus.Core.EventBus();
    var newsEventBus = new EventBus.Core.EventBus();
    var cts = new CancellationTokenSource();

    Console.WriteLine("Setting up Sensor Monitoring System...");

    // Setup sensors and displays
    var alertDisplay = new AlertDisplay("Main");
    var numericDisplay = new NumericDisplay("All");
    sensorEventBus.Register(alertDisplay);
    sensorEventBus.Register(numericDisplay);

    var tempSensor = new TemperatureSensor("TEMP-001", "North", sensorEventBus);
    var waterSensor = new WaterLevelSensor("WATER-001", "Delta", sensorEventBus);

    Console.WriteLine("Setting up News Agency System...");

    // Setup news agencies and subscribers
    var reader = new Person("Reader", NewsCategory.Technology, NewsCategory.Sports);
    var archive = new NewsArchive();
    newsEventBus.Register(reader);
    newsEventBus.Register(archive);

    var techAgency = new TechnologyNewsAgency(newsEventBus);
    var sportsAgency = new SportsNewsAgency(newsEventBus);

    Console.WriteLine("\nStarting both systems... (Press any key to stop)\n");
    Console.WriteLine("--------------------------------------------------\n");

    // Start all components
    var tasks = new[]
    {
        tempSensor.StartAsync(cts.Token),
        waterSensor.StartAsync(cts.Token),
        techAgency.StartPublishingAsync(cts.Token),
        sportsAgency.StartPublishingAsync(cts.Token)
    };

    // Wait for user input to stop
    await Task.Run(() => Console.ReadKey(true));

    Console.WriteLine("\n\nStopping all systems...");
    cts.Cancel();

    try
    {
        await Task.WhenAll(tasks);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("All systems stopped successfully");
    }

    Console.WriteLine("\n-----------------------------------------------------------");
    Console.WriteLine("This demonstrates that EventBus can handle multiple");
    Console.WriteLine("independent systems simultaneously with complete isolation!");
    Console.WriteLine("n-----------------------------------------------------------\n");

    archive.DisplayArchiveSummary();

    Console.WriteLine("\nPress any key to return to menu...");
    Console.ReadKey();
}