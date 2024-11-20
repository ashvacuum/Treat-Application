# Quick Start Guide

To be able to test properly ensure you:
1. open the **InitializationScene**
2. Press Play and Start working Testing the game that way
3. Enter a name for the game to track the scores

The game is setup so that it relies on Firestore to store data and leaderboards


# Design Decisions

Tried to Follow SOLID Design Patterns as closely as possible with a few design patterns sprinkled in to follow SOLID more quickly as well as the reasoning for using them listed out below:

- [EventBus Decision](#eventbus-design-pattern-&-solid-principles)
- [Service Locator Decision](#service-locator-design-considerations)


# EventBus Design Pattern & SOLID Principles

## Overview
An EventBus acts as a mediator for communication between components in a system, implementing a publish-subscribe pattern without direct coupling between components.

## Why Use EventBus?

### 1. Single Responsibility Principle (S)
- **Decouples Event Handling**: Each component only needs to:
  - *Publish* events it's responsible for
  - *Subscribe* to events it needs to react to
- **Focused Components**: Components don't need to know about each other's existence

### 2. Open/Closed Principle (O)
- **Extensible Design**: New subscribers can be added without modifying existing code
- **Event Evolution**: 
  - New event types can be introduced
  - Existing subscribers remain unaffected
  - System is open for extension, closed for modification

### 3. Liskov Substitution Principle (L)
- **Event Type Hierarchy**: 
  - Events can inherit from base event types
  - Subscribers can handle base or derived events
  - Example:
    ```csharp
    class GameEvent{}
    class GameEndEvent : GameEvent{}
    class GameQuitEvent : GameEvent{}
    ```

### 4. Interface Segregation Principle (I)
- **Minimal Interfaces**: 
  - Publishers only need publish method
  - Subscribers only need subscribe method
  - No forced implementation of unnecessary methods

### 5. Dependency Inversion Principle (D)
- **Abstraction**: Components depend on abstract EventBus interface
- **Flexibility**: Easy to:
  - Switch EventBus implementations
  - Mock for testing
  - Use Firebase 

## Practical Example
```csharp
void Start() {
    EventBus.Subscribe<GameStartEvent>(OnGameStart);
}

private void OnGameStart(GameStartEvent evt){
	// use game start event data
}

private void StartGame(){
	EventBus.Publish(new GameStartEvent());
}

public class GameStartEvent : GameEvent  
{  
	  public float StartTime { get; }  
	  public int Difficulty { get; }  
	  public string Username { get; }  
	  public GameStartEvent(float startTime, int difficulty, string username)
	  {  
			StartTime = startTime;
			Difficulty = difficulty;
			Username = username;
	   }
}

```

# Service Locator Design Considerations
## Overview
ServiceLocator acts as a centralized registry for accessing application services, particularly useful with Firebase services. This document explores how it aligns with SOLID principles and its benefits in Firebase integration.

## SOLID Principles Alignment

### Single Responsibility Principle (S)
- ServiceLocator's sole responsibility is managing service instances
- Each service can handle a specific Firebase feature 
- Clear separation between service registration and service consumption
- Simplified dependency management in components

### Open/Closed Principle (O)
- New services can be added without modifying ServiceLocator code
- Existing services remain unchanged when adding new functionality
- Service interfaces can be extended for new features
- Support for plugin-style architecture

### Liskov Substitution Principle (L)
- Service implementations are interchangeable
- Mock services can replace real Firebase services
- Testing configurations work seamlessly with production code

### Interface Segregation Principle (I)
- Service interfaces are focused and minimal
- Clients only depend on methods they use
- Firebase services split into logical interface groups
- Prevents unnecessary coupling to unused Firebase features

### Dependency Inversion Principle (D)
- Components depend on abstractions (interfaces)
- Implementation details isolated from business logic
- Easy to switch between different Firebase implementations
- Supports different configurations per environment

## Example Initialization
```csharp
private async void Awake()  
{  
  var firestoreService = new FirestoreService();  
  await firestoreService.Initialize();  
  ServiceLocator.Instance.RegisterService<IFirestoreService>(firestoreService); 
}  
  
private void OnDestroy()  
{  
  ServiceLocator.Instance.Reset();  
}
```

## Example Use
```csharp
private IFirestoreService firestoreService; 
  
private void Awake()  
{  
  firestoreService = ServiceLocator.Instance.GetService<IFirestoreService>();  
}
```

## Implementable Interface for Firestore
```csharp
public interface IFirestoreService  
{  
  Task<bool> Initialize();  
  Task SavePlayerScore(string playerId, string playerName, int score, int difficulty);  
  Task<Dictionary<string, object>> GetPlayerData(string playerId);  
  Task UpdatePlayerScore(string playerId, int newScore);  
  Task<List<GameSessionData>> QueryHighScores(int minimumScore);  
}
```


