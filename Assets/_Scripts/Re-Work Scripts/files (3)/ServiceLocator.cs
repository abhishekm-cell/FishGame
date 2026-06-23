using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private static ServiceLocator _instance;
    public static ServiceLocator Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("[ServiceLocator] No ServiceLocator in scene.");
            return _instance;
        }
    }

    private readonly Dictionary<Type, object> _services = new();

    void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
    }

    public void Register<T>(T service)
    {
        var type = typeof(T);
        if (_services.ContainsKey(type))
            Debug.LogWarning($"[ServiceLocator] Overwriting existing service: {type.Name}");
        _services[type] = service;
    }

    public T Get<T>()
    {
        if (_services.TryGetValue(typeof(T), out var service))
            return (T)service;
        Debug.LogError($"[ServiceLocator] Service not found: {typeof(T).Name}");
        return default;
    }

    public bool TryGet<T>(out T service)
    {
        if (_services.TryGetValue(typeof(T), out var raw))
        {
            service = (T)raw;
            return true;
        }
        service = default;
        return false;
    }

    public void Unregister<T>() => _services.Remove(typeof(T));

    void OnDestroy()
    {
        _services.Clear();
        if (_instance == this) _instance = null;
    }
}
