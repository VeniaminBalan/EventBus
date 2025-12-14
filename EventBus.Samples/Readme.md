# EventBus.Samples

This project contains two comprehensive sample applications that demonstrate the flexibility, power, and generality of the EventBus library. Both examples showcase real-world scenarios where loose coupling between publishers and subscribers is essential.

## Overview

The EventBus library enables decoupled communication between components without requiring them to have direct references to each other. These sample applications illustrate how the same EventBus infrastructure can be applied to completely different domains, demonstrating its versatility and reusability.

## Sample Application 1: Sensor Monitoring System

### Description

A distributed sensor monitoring system where multiple types of sensors publish data to various display types. This example demonstrates:
- **Multiple event types** (temperature, humidity, water level readings)
- **Multiple subscriber instances** of the same type
- **Event filtering** based on sensor properties (e.g., region, threshold values)
- **Priority-based processing** for critical alerts
- **Real-time data streaming** from simulated sensors

### Architecture

#### Publishers (Sensors)
- **TemperatureSensor** - Publishes temperature readings with location data
- **WaterLevelSensor** - Publishes water level measurements
- **HumiditySensor** - Publishes humidity readings
- **PressureSensor** - Publishes atmospheric pressure data

Each sensor type:
- Runs as a simulated data source with periodic readings
- Includes metadata (sensor ID, location/region, timestamp)
- Can generate alert events when readings exceed thresholds

#### Subscribers (Displays)
- **NumericDisplay** - Shows current numerical values from sensors
- **AverageDisplay** - Calculates and displays running averages
- **DetailedDisplay** - Shows comprehensive sensor information with history
- **AlertDisplay** - Monitors for critical values and displays warnings

Each display type can:
- Subscribe to specific sensor types
- Filter events based on region or other criteria
- Process multiple sensor streams simultaneously
- Update visualization in real-time

#### Events
```csharp
- TemperatureReadingEvent (temperature, sensorId, region, timestamp)
- WaterLevelReadingEvent (level, sensorId, location, timestamp)
- HumidityReadingEvent (humidity, sensorId, region, timestamp)
- SensorAlertEvent (sensorType, severity, value, threshold, region)
```

### Key Features Demonstrated
- **Loose Coupling**: Sensors have no knowledge of which displays exist
- **Flexibility**: New display types can be added without modifying sensors
- **Filtering**: Displays can selectively process events based on custom criteria
- **Scalability**: Multiple sensor and display instances can coexist
- **Event Priority**: Critical alerts are processed before regular readings

### Use Cases
- Environmental monitoring systems
- Industrial IoT platforms
- Smart building management
- Agricultural monitoring
- Weather station networks

---

## Sample Application 2: News Agency System

### Description

A multi-agency news distribution system where news agencies publish articles and subscribers receive news based on their interests. This example demonstrates:
- **Multiple publishers** (different news agencies)
- **Category-based filtering** (sports, politics, culture, technology)
- **Dynamic subscriptions** (users can change their interests at runtime)
- **Publisher identification** (tracking which agency published what)
- **Subscription management** (subscribe/unsubscribe from domains)

### Architecture

#### Publishers (News Agencies)
- **SportsNewsAgency** - Publishes sports-related news
- **PoliticsNewsAgency** - Publishes political news
- **CultureNewsAgency** - Publishes cultural events and entertainment
- **TechnologyNewsAgency** - Publishes tech and science news
- **GeneralNewsAgency** - Publishes news across multiple domains

Each agency:
- Publishes news articles in one or more domains
- Includes agency branding and metadata
- Can publish breaking news with high priority
- Maintains editorial independence

#### Subscribers (People/News Readers)
- **Person** - Represents an individual news consumer
- **NewsAggregator** - Collects news from all domains
- **SpecializedReader** - Subscribes to specific combinations of domains
- **NewsArchive** - Stores all published news for historical purposes

Each subscriber can:
- Subscribe to multiple news domains simultaneously
- Change subscriptions dynamically at runtime
- Filter news by agency, domain, or keywords
- Receive notifications for breaking news

#### Events
```csharp
- SportsNewsEvent (headline, content, agency, timestamp, priority)
- PoliticsNewsEvent (headline, content, agency, region, timestamp)
- CultureNewsEvent (headline, content, agency, category, timestamp)
- TechnologyNewsEvent (headline, content, agency, topic, timestamp)
- BreakingNewsEvent (headline, content, agency, domain, urgency)
```

### Key Features Demonstrated
- **Multi-Domain Publishing**: Single agency publishes to multiple categories
- **Dynamic Subscriptions**: Subscribers change interests at runtime
- **Event Categorization**: News organized by domain/category
- **Publisher Diversity**: Multiple independent publishers
- **Priority Handling**: Breaking news processed with higher priority
- **Subscriber Flexibility**: Different subscription patterns per user

### Use Cases
- News aggregation platforms
- RSS feed systems
- Social media content distribution
- Corporate communication systems
- Information broadcast networks

---

## How These Examples Demonstrate EventBus Generality

### Common Patterns Across Both Applications

1. **Publisher-Subscriber Decoupling**
   - Publishers (sensors/agencies) don't know about subscribers (displays/people)
   - Subscribers can be added/removed without affecting publishers
   - No direct dependencies between components

2. **Event-Driven Architecture**
   - All communication happens through events
   - Events are strongly typed
   - Events carry relevant data payloads

3. **Dynamic Registration**
   - Subscribers can register/unregister at runtime
   - System adapts to changing subscriber sets
   - No configuration files needed

4. **Type Safety**
   - Each subscriber specifies exact event types it handles
   - Compile-time type checking
   - No casting or type checking needed

5. **Flexibility**
   - Same EventBus infrastructure serves both applications
   - No application-specific modifications to EventBus
   - Easy to extend with new event types

### Differences Highlighting Versatility

| Aspect | Sensor Monitoring | News Agency |
|--------|------------------|-------------|
| **Data Flow** | Continuous streaming | Discrete publications |
| **Event Frequency** | High (real-time) | Low to medium (periodic) |
| **Filtering** | Region, sensor type | Domain, agency, topic |
| **Priority Use** | Critical alerts | Breaking news |
| **Subscribers** | Technical displays | Human readers |
| **Event Lifetime** | Transient readings | Archivable articles |

---

## Running the Samples

### Prerequisites
- .NET 10.0 or later
- EventBus.Core library reference

### Sensor Monitoring System
```bash
dotnet run --project SensorMonitoring
```

The application will:
1. Initialize multiple sensors of different types
2. Create various display instances
3. Start sensors publishing readings every 1-2 seconds
4. Show real-time updates on all displays
5. Generate alerts when thresholds are exceeded

### News Agency System
```bash
dotnet run --project NewsAgency
```

The application will:
1. Create multiple news agencies
2. Initialize news readers with different interests
3. Publish news articles periodically
4. Demonstrate dynamic subscription changes
5. Show breaking news with priority handling

---

## Learning Objectives

By studying these examples, you will learn how to:

1. **Design event-driven systems** using the EventBus pattern
2. **Implement publishers** that broadcast events without knowing subscribers
3. **Create subscribers** that react to specific event types
4. **Use filtering** to process only relevant events
5. **Handle priorities** for important events
6. **Manage dynamic subscriptions** at runtime
7. **Apply the same pattern** to different problem domains
8. **Build loosely-coupled** and maintainable applications

---

## Code Structure

```
EventBus.Samples/
├── SensorMonitoring/
│   ├── Sensors/
│   │   ├── TemperatureSensor.cs
│   │   ├── HumiditySensor.cs
│   │   ├── WaterLevelSensor.cs
│   │   └── BaseSensor.cs
│   ├── Displays/
│   │   ├── NumericDisplay.cs
│   │   ├── AverageDisplay.cs
│   │   ├── DetailedDisplay.cs
│   │   └── AlertDisplay.cs
│   ├── Events/
│   │   ├── TemperatureReadingEvent.cs
│   │   ├── HumidityReadingEvent.cs
│   │   ├── WaterLevelReadingEvent.cs
│   │   └── SensorAlertEvent.cs
│   └── Program.cs
│
├── NewsAgency/
│   ├── Agencies/
│   │   ├── SportsNewsAgency.cs
│   │   ├── PoliticsNewsAgency.cs
│   │   ├── CultureNewsAgency.cs
│   │   └── BaseNewsAgency.cs
│   ├── Subscribers/
│   │   ├── Person.cs
│   │   ├── NewsAggregator.cs
│   │   └── NewsArchive.cs
│   ├── Events/
│   │   ├── NewsArticleEvent.cs
│   │   ├── BreakingNewsEvent.cs
│   │   └── NewsCategory.cs
│   └── Program.cs
│
└── Readme.md (this file)
```

---

## Extension Ideas

### For Sensor Monitoring
- Add graphical visualization of sensor data
- Implement data persistence to database
- Add sensor calibration events
- Create historical trend analysis
- Implement sensor health monitoring

### For News Agency
- Add user preferences and personalization
- Implement news search and filtering
- Create news recommendation system
- Add comments and engagement tracking
- Implement news expiration and archival

---

## Conclusion

These two sample applications demonstrate that the EventBus library is a general-purpose solution for event-driven architecture. The same core library seamlessly handles both continuous sensor data streams and discrete news article publications, proving its versatility across different domains.

The key takeaway is that **EventBus provides a clean, type-safe, and flexible foundation for building decoupled systems**, regardless of the application domain.
