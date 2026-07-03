using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registered as a service in ServiceLocator by GameBootstrapper.
/// FishController and HookController inject it via ServiceLocator.Get.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private FishDataSO fishData;
    [SerializeField] private GameObject hookPrefab;
 
    [Header("Pool sizes")]
    [SerializeField] private int fishPoolSizePerTier = 6;
    [SerializeField] private int hookPoolSize = 6;
 
    private readonly Dictionary<int, Queue<GameObject>> _fishPools = new();
    private readonly Queue<GameObject> _hookPool = new();
 
    void Awake()
    {
        foreach (var tier in fishData.tiers)
        {
            var pool = new Queue<GameObject>();
            for (int i = 0; i < fishPoolSizePerTier; i++)
            {
                var obj = Instantiate(tier.preFab, transform);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            _fishPools[tier.sizeValue] = pool;
        }
 
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
 
    public GameObject GetFish(int sizeValue)
    {
        if (!_fishPools.TryGetValue(sizeValue, out var pool))
        {
            Debug.LogWarning($"[ObjectPool] No pool for sizeValue={sizeValue}, check FishDataSO tiers/prefabs.");
            return null;
        }
 
        if (pool.Count > 0) return pool.Dequeue();
 
        var tier = fishData.GetTier(sizeValue);
        return Instantiate(tier.preFab, transform);
    }
 
    public void ReturnFish(GameObject obj, int sizeValue)
    {
        obj.SetActive(false);
        if (_fishPools.TryGetValue(sizeValue, out var pool))
        {
            pool.Enqueue(obj);
        }
        else
        {
            Debug.LogWarning($"[ObjectPool] ReturnFish: no pool for sizeValue={sizeValue}, destroying instead.");
        }

    }
 
    public GameObject GetHook()
    {
        if (_hookPool.Count > 0) return _hookPool.Dequeue();
        var obj = Instantiate(hookPrefab, transform);
        
        return obj;
    }
 
    public void ReturnHook(GameObject obj)
    {
        obj.SetActive(false);
        _hookPool.Enqueue(obj);
    }
}
