using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _listeners = new();

    public static void Subscribe<T>(Action<T> listener)
    {
        var type = typeof(T);
        if (!_listeners.ContainsKey(type))
            _listeners[type] = new List<Delegate>();
        _listeners[type].Add(listener);
    }

    public static void Unsubscribe<T>(Action<T> listener)
    {
        var type = typeof(T);
        if (_listeners.ContainsKey(type))
            _listeners[type].Remove(listener);
    }

    public static void Publish<T>(T evt)
    {
        var type = typeof(T);
        if (!_listeners.ContainsKey(type)) return;
        foreach (var d in _listeners[type].ToArray())
            (d as Action<T>)?.Invoke(evt);
    }

    public static void Clear() => _listeners.Clear();
}

// ── Events ────────────────────────────────────────────────────────────
public struct OnPlayerAte    { public int eatenFishSize; public int newPlayerSize; }
public struct OnPlayerDied   { public string reason; }  // "fish" or "hook"
public struct OnHookCaught   { public Transform hookTransform; }
public struct OnScoreChanged { public int score; }
public struct OnGameStarted  { }
public struct OnGameOver     { public string reason; public int finalScore; }
