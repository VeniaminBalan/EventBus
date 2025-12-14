# EventBus Architecture

## Overview

A general-purpose EventBus infrastructure for .NET that enables loose coupling between publishers and subscribers within the same process using reflection and attributes.

## Core Principles

- **Loose Coupling**: Publishers don't know subscribers; subscribers don't know publishers
- **Type Safety**: Events are strongly typed through method parameters
- **Flexibility**: No predefined event types; applications define their own
- **Simplicity**: No mandatory interfaces for subscribers
- **Reflection-Based**: Automatic discovery of handler methods

## Architecture Components

### 1. Core Components

#### EventBus (Central Hub)
- **Responsibility**: Manages the relationship between publishers and subscribers
- **Key Operations**:
  - `Register(object subscriber)` - Register an object as a subscriber
  - `Unregister(object subscriber)` - Remove a subscriber
  - `Publish(object event)` - Publish an event to all relevant subscribers
  - `PublishAsync(object event)` - Asynchronous event publishing

#### EventHandlerAttribute
- **Purpose**: Marks methods as event handlers
- **Usage**: Applied to methods that should receive events
- **Properties**:
  - `Priority` (optional) - Execution order of handlers
  - `ThreadMode` (optional) - Execution thread context

#### SubscriberRegistry
- **Responsibility**: Internal registry that maps event types to subscribers
- **Data Structure**: `Dictionary<Type, List<SubscriberMethod>>`
- **Operations**:
  - Add subscriber methods
  - Remove subscriber methods
  - Lookup handlers by event type

#### SubscriberMethod
- **Purpose**: Encapsulates information about a handler method
- **Properties**:
  - `object Subscriber` - The subscriber instance
  - `MethodInfo Method` - Reflection info about the handler method
  - `Type EventType` - The type of event this handler accepts
  - `int Priority` - Handler execution priority
  - `ThreadMode ThreadMode` - Threading behavior

### 2. Event Model

#### Base Event (Optional)
```csharp
public abstract class Event
{
    public DateTime Timestamp { get; }
    public Guid EventId { get; }
}
```

**Note**: Events can be ANY type; base class is optional for convenience.

#### Custom Events
Applications define their own event types:
```csharp
public class UserLoggedInEvent
{
    public string Username { get; set; }
    public DateTime LoginTime { get; set; }
}

public class OrderPlacedEvent
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
}
```

### 3. Threading Models

#### ThreadMode Enum
- **Posting** - Handler executes on the publishing thread (default)
- **Background** - Handler executes on a background thread
- **Async** - Handler executes asynchronously
- **MainThread** - Handler executes on the main/UI thread (if applicable)

### 4. Exception Handling

#### EventBusException
- Base exception for EventBus-specific errors

#### SubscriberExceptionEvent
- Special event published when a handler throws an exception
- Allows centralized error handling

## Component Interaction Flow

### Registration Flow
```
1. Application calls EventBus.Register(subscriberObject)
2. EventBus uses reflection to scan subscriberObject
3. Find all methods with [EventHandler] attribute
4. Validate methods (must have exactly one parameter)
5. Extract event type from parameter type
6. Create SubscriberMethod instances
7. Add to SubscriberRegistry indexed by event type
```

### Publishing Flow
```
1. Publisher calls EventBus.Publish(eventObject)
2. EventBus determines event.GetType()
3. Query SubscriberRegistry for matching handlers
4. Sort handlers by priority
5. For each handler:
   a. Respect ThreadMode setting
   b. Invoke method via reflection
   c. Pass eventObject as parameter
   d. Handle exceptions according to policy
```

### Unregistration Flow
```
1. Application calls EventBus.Unregister(subscriberObject)
2. EventBus finds all SubscriberMethod instances for that object
3. Remove from SubscriberRegistry
4. Clear references to allow garbage collection
```

## Class Diagram (Conceptual)

```
┌─────────────────┐
│   EventBus      │
│─────────────────│
│ - registry      │
│─────────────────│
│ + Register()    │
│ + Unregister()  │
│ + Publish()     │
│ + PublishAsync()│
└────────┬────────┘
         │
         │ uses
         ▼
┌─────────────────────┐
│ SubscriberRegistry  │
│─────────────────────│
│ - handlers: Dict    │
│─────────────────────│
│ + AddHandler()      │
│ + RemoveHandler()   │
│ + GetHandlers()     │
└─────────────────────┘
         │
         │ contains
         ▼
┌─────────────────────┐
│  SubscriberMethod   │
│─────────────────────│
│ + Subscriber        │
│ + Method            │
│ + EventType         │
│ + Priority          │
│ + ThreadMode        │
│─────────────────────│
│ + Invoke()          │
└─────────────────────┘
```

## Usage Example

### Subscriber Definition
```csharp
public class UserService
{
    [EventHandler]
    public void OnUserLoggedIn(UserLoggedInEvent evt)
    {
        Console.WriteLine($"User {evt.Username} logged in");
    }

    [EventHandler(Priority = 1)]
    public void OnOrderPlaced(OrderPlacedEvent evt)
    {
        // High priority handler
        ValidateOrder(evt.OrderId);
    }
}
```

### Publisher Usage
```csharp
// Registration
var userService = new UserService();
EventBus.Default.Register(userService);

// Publishing
EventBus.Default.Publish(new UserLoggedInEvent 
{ 
    Username = "john.doe",
    LoginTime = DateTime.Now 
});

// Cleanup
EventBus.Default.Unregister(userService);
```

## Design Patterns Used

### 1. Observer Pattern
- EventBus acts as the subject
- Subscribers act as observers
- Events are the notifications

### 2. Singleton Pattern
- `EventBus.Default` provides a default instance
- Allows application-wide event bus
- Also supports creating custom instances

### 3. Registry Pattern
- SubscriberRegistry maintains mappings
- Efficient lookup of handlers by event type

### 4. Command Pattern (Optional)
- Events can encapsulate commands
- Handlers execute the command logic

## Performance Considerations

### Reflection Caching
- Handler methods discovered once during registration
- MethodInfo cached in SubscriberMethod
- No reflection during event publishing (except invoke)

### Thread Safety
- EventBus must be thread-safe
- Use `ConcurrentDictionary` for registry
- Lock-free reads where possible
- Write operations properly synchronized

### Memory Management
- Use `WeakReference` for subscribers (optional)
- Prevents memory leaks from forgotten unregistrations
- Trade-off: slight complexity increase

## Advanced Features

### 1. Inheritance Support
- Handlers can receive base event types
- Automatically match derived event types
- Example: `EventHandler<Event>` receives all events inheriting from Event

### 2. Sticky Events
- Events that persist after publishing
- New subscribers immediately receive last sticky event
- Useful for configuration/state events

### 3. Event Filtering
- Subscribers can specify filter conditions
- Only receive events matching filter
- Implementation: `[EventHandler(Filter = "condition")]`

### 4. Dead Event Handling
- Special event for when no handlers found
- Useful for debugging
- `DeadEvent` wrapper class

### 5. Async Support
- Async event handlers: `Task OnEventAsync(MyEvent evt)`
- EventBus.PublishAsync() returns Task
- Proper async/await support

## Error Handling Strategy

### Handler Exceptions
1. Catch exceptions in individual handlers
2. Log exception details
3. Publish `SubscriberExceptionEvent`
4. Continue invoking other handlers
5. Option: Stop on first exception (configurable)

### Registration Errors
1. Validate handler methods during registration
2. Throw `EventBusException` for invalid handlers
3. Provide clear error messages

## Testing Strategy

### Unit Testing
- Mock subscribers for isolation
- Verify handler invocation
- Test registration/unregistration
- Test exception handling

### Integration Testing
- Test real subscriber objects
- Verify event flow
- Test threading behavior
- Performance benchmarks

## Project Structure

```
EventBus.Core/
├── EventBus.cs                    # Main EventBus class
├── Attributes/
│   ├── EventHandlerAttribute.cs   # Handler method attribute
│   └── SubscribeAttribute.cs      # Alternative naming
├── Models/
│   ├── SubscriberMethod.cs        # Handler method wrapper
│   ├── Event.cs                   # Optional base event
│   └── DeadEvent.cs               # No handler event
├── Registry/
│   ├── SubscriberRegistry.cs      # Handler registry
│   └── EventTypeCache.cs          # Type hierarchy cache
├── Enums/
│   └── ThreadMode.cs              # Threading options
├── Exceptions/
│   ├── EventBusException.cs       # Base exception
│   └── SubscriberExceptionEvent.cs # Handler error event
└── Interfaces/
    └── IEventBus.cs               # EventBus contract (optional)
```

## Implementation Phases

### Phase 1: Core Functionality
- [x] Basic EventBus class
- [x] EventHandlerAttribute
- [x] Registration mechanism
- [x] Synchronous publishing
- [x] Simple subscriber registry

### Phase 2: Enhanced Features
- [ ] Threading support (ThreadMode)
- [ ] Priority-based execution
- [ ] Exception handling
- [ ] Async publishing

### Phase 3: Advanced Features
- [ ] Inheritance support
- [ ] Sticky events
- [ ] Weak references
- [ ] Performance optimizations

### Phase 4: Production Ready
- [ ] Comprehensive unit tests
- [ ] Integration tests
- [ ] Performance benchmarks
- [ ] Documentation
- [ ] NuGet package

## Configuration Options

```csharp
public class EventBusConfiguration
{
    public bool ThrowSubscriberException { get; set; } = false;
    public bool SendSubscriberExceptionEvent { get; set; } = true;
    public bool LogSubscriberExceptions { get; set; } = true;
    public bool LogNoSubscriberMessages { get; set; } = false;
    public bool SendNoSubscriberEvent { get; set; } = false;
    public bool UseWeakReferences { get; set; } = false;
    public int EventInheritanceDepth { get; set; } = int.MaxValue;
}
```

## Next Steps

1. Implement core EventBus class
2. Create EventHandlerAttribute
3. Build SubscriberRegistry
4. Implement registration logic with reflection
5. Implement publishing mechanism
6. Add unit tests
7. Create sample applications
8. Performance optimization
9. Documentation and examples
