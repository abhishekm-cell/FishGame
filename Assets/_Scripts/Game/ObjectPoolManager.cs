using System.Collections.Generic;
using System;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    [Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int initialSize = 10;
    }

    [Serializable]
    public class ObstaclePool
    {
        public GameObject prefab;
        public int initialSize = 10;
    }

    [SerializeField] private Pool[] pools;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            poolDictionary.Add(pool.prefab, queue);
        }

    }

    private void OnEnable()
    {
        Events.RequestSpawn += HandleSpawn;
        Events.RequestDespawn += HandleDespawn;
    }

    private void OnDisable()
    {
        Events.RequestSpawn -= HandleSpawn;
        Events.RequestDespawn -= HandleDespawn;
    }

    void HandleSpawn(GameObject prefab, Vector3 pos, Quaternion rot, Action<GameObject> callback)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning($"No pool found for {prefab.name}");
            return;
        }

        GameObject obj = poolDictionary[prefab].Count > 0
            ? poolDictionary[prefab].Dequeue()
            : Instantiate(prefab, transform);

        obj.transform.SetPositionAndRotation(pos, rot);
        obj.SetActive(true);

        callback?.Invoke(obj);
    }

    void HandleDespawn(GameObject instance, GameObject prefab)
    {
        instance.SetActive(false);
        poolDictionary[prefab].Enqueue(instance);
    }
}

public class Demodata
{
    public GameObject prefab;
    public Vector3 pos;
    public Quaternion rot;
}


         
