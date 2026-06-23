using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registered as a service in ServiceLocator by GameBootstrapper.
/// FishController and HookController inject it via ServiceLocator.Get.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject fishPrefab;
    [SerializeField] private GameObject hookPrefab;

    [Header("Pool sizes")]
    [SerializeField] private int fishPoolSize = 20;
    [SerializeField] private int hookPoolSize = 6;

    private readonly Queue<GameObject> _fishPool = new();
    private readonly Queue<GameObject> _hookPool = new();

    void Awake()
    {
        Prewarm(_fishPool, fishPrefab, fishPoolSize);
        Prewarm(_hookPool, hookPrefab, hookPoolSize);
    }

    private void Prewarm(Queue<GameObject> pool, GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetFish()
    {
        if (_fishPool.Count > 0) return _fishPool.Dequeue();
        var obj = Instantiate(fishPrefab, transform);
        //obj.SetActive(false);
        return obj;
    }

    public void ReturnFish(GameObject obj)
    {
        obj.SetActive(false);
        _fishPool.Enqueue(obj);
    }

    public GameObject GetHook()
    {
        if (_hookPool.Count > 0) return _hookPool.Dequeue();
        var obj = Instantiate(hookPrefab, transform);
        //obj.SetActive(false);
        return obj;
    }

    public void ReturnHook(GameObject obj)
    {
        obj.SetActive(false);
        _hookPool.Enqueue(obj);
    }
}
