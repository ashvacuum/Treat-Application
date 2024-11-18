using UnityEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

// Event base class
public abstract class GameEvent { }

// Specific event types
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

public class GameQuitEvent : GameEvent
{
    
}

public class TimerStartEvent : GameEvent
{
    public float TimeLeft { get; }

    public TimerStartEvent(float timeLeft)
    {
        TimeLeft = timeLeft;
    }
}

public class TimerUpdateEvent : GameEvent
{
    public float TimeLeft { get; }

    public TimerUpdateEvent(float timeLeft)
    {
        TimeLeft = timeLeft;
    }
}


public class GamePauseEvent : GameEvent
{
    public bool IsPaused { get; }

    public GamePauseEvent(bool isPaused)
    {
        IsPaused = isPaused;
    }
}

//for analytics usage
public class GameEndEvent : GameEvent
{
    public int FinalScore { get; }
    public float GameDuration { get; }
    public int MovesLeft { get; }
    public bool DidWin { get; }
    
    public GameEndEvent(int finalScore, float gameDuration, int movesLeft, bool didWin)
    {
        FinalScore = finalScore;
        GameDuration = gameDuration;
        MovesLeft = movesLeft;
        DidWin = didWin;
    }
}

public class MoveChangedEvent : GameEvent
{
    public int MoveCount { get; }

    public MoveChangedEvent(int moveCount)
    {
        MoveCount = moveCount;
    }
}

public class ScoreChangedEvent : GameEvent
{
    public int NewScore { get; }
    public int ScoreDelta { get; }
    
    public ScoreChangedEvent(int newScore, int scoreDelta)
    {
        NewScore = newScore;
        ScoreDelta = scoreDelta;
    }
}

// Event Bus
public static class EventBus
{
    private static Dictionary<Type, List<Action<GameEvent>>> eventActions =
        new Dictionary<Type, List<Action<GameEvent>>>();

    public static void Subscribe<T>(Action<T> action) where T : GameEvent
    {
        Type eventType = typeof(T);
        if (!eventActions.ContainsKey(eventType))
        {
            eventActions[eventType] = new List<Action<GameEvent>>();
        }

        eventActions[eventType].Add(e => action((T)e));
    }

    public static void Unsubscribe<T>(Action<T> action) where T : GameEvent
    {
        Type eventType = typeof(T);
        if (eventActions.ContainsKey(eventType))
        {
            eventActions[eventType].Remove(e => action((T)e));
        }
    }

    public static void Publish<T>(T gameEvent) where T : GameEvent
    {
        Type eventType = typeof(T);
        if (eventActions.ContainsKey(eventType))
        {
            foreach (var action in eventActions[eventType])
            {
                action(gameEvent);
            }
        }
    }
}


