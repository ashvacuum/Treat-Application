using UnityEngine;
using System;
using System.Collections.Generic;

// Event base class
public abstract class GameEvent { }

// Specific event types
public class GameStartEvent : GameEvent
{
    public float StartTime { get; }
    public int Difficulty { get; }
    public GameStartEvent(float startTime, int difficulty)
    {
        StartTime = startTime;
        Difficulty = difficulty;
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
    private static readonly Dictionary<Type, Dictionary<object, Action<GameEvent>>> subscribers 
        = new Dictionary<Type, Dictionary<object, Action<GameEvent>>>();

    public static void Subscribe<T>(Action<T> subscriber) where T : GameEvent
    {
        var eventType = typeof(T);
        
        if (!subscribers.ContainsKey(eventType))
        {
            subscribers[eventType] = new Dictionary<object, Action<GameEvent>>();
        }

        void Wrapper(GameEvent e) => subscriber((T)e);
        subscribers[eventType][subscriber] = Wrapper;
    }

    public static void Unsubscribe<T>(Action<T> subscriber) where T : GameEvent
    {
        var eventType = typeof(T);
        
        if (subscribers.ContainsKey(eventType))
        {
            if (subscribers[eventType].ContainsKey(subscriber))
            {
                subscribers[eventType].Remove(subscriber);
                
                if (subscribers[eventType].Count == 0)
                {
                    subscribers.Remove(eventType);
                }
            }
        }
    }

    public static void Publish<T>(T gameEvent) where T : GameEvent
    {
        var eventType = typeof(T);
        
        if (subscribers.ContainsKey(eventType))
        {
            // Create a copy of the delegates to avoid modification during iteration
            var delegates = new List<Action<GameEvent>>(subscribers[eventType].Values);
            
            foreach (var subscriber in delegates)
            {
                try
                {
                    subscriber.Invoke(gameEvent);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error publishing event {eventType.Name}: {e.Message}");
                }
            }
        }
    }

    public static int GetSubscriberCount<T>() where T : GameEvent
    {
        var eventType = typeof(T);
        return subscribers.ContainsKey(eventType) ? subscribers[eventType].Count : 0;
    }

    public static void Clear()
    {
        subscribers.Clear();
    }
}


