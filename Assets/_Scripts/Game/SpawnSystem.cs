using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnSystem : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private LaneManager laneManager;
    [SerializeField] private FoodData[] foodPrefab;

    [SerializeField] private float spawnX = 10f;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawntime = 1f;
    
    [Header("Obstacle Settings")]
    [SerializeField] private float obstacleSpawnInterval = 5f;
    [SerializeField] private float obstacleSpawnTime = 3f;
    [SerializeField] private ObstacleData obstacleData;
    
    [Header("Spawn Limits")]
    [SerializeField] private int maxFoodOnScreen = 10;
    [SerializeField] private int maxObstaclesOnScreen = 3;
    
    [Header("References")]
    [SerializeField] private GameManager gManager;

    // Coroutine references for control
    private Coroutine foodSpawnCoroutine;
    private Coroutine obstacleSpawnCoroutine;
    
    // Track active spawns
    private int activeFoodCount = 0;
    private int activeObstacleCount = 0;
    
    // WaitForSeconds caching for optimization
    private WaitForSeconds foodWaitTime;
    private WaitForSeconds obstacleWaitTime;

    void Awake()
    {
        // Cache WaitForSeconds to avoid allocation every frame
        foodWaitTime = new WaitForSeconds(spawnInterval);
        obstacleWaitTime = new WaitForSeconds(obstacleSpawnInterval);
    }

    public void SetReference(GameManager gameManager)
    {
        gManager = gameManager;
    }

    void OnEnable()
    {
        Events.GameStart += StartSpawning;
        Events.ResetGame += StopSpawning;
    }

    void OnDisable()
    {
        Events.GameStart -= StartSpawning;
        Events.ResetGame -= StopSpawning;
    }

    public void StartSpawning()
    {
        StopSpawning(); // Stop any existing coroutines first
        
        foodSpawnCoroutine = StartCoroutine(FoodSpawnRoutine());
        obstacleSpawnCoroutine = StartCoroutine(ObstacleSpawnRoutine());
    }

    public void StopSpawning()
    {
        if (foodSpawnCoroutine != null)
        {
            StopCoroutine(foodSpawnCoroutine);
            foodSpawnCoroutine = null;
        }
        
        if (obstacleSpawnCoroutine != null)
        {
            StopCoroutine(obstacleSpawnCoroutine);
            obstacleSpawnCoroutine = null;
        }
    }

    private IEnumerator FoodSpawnRoutine()
    {
        
        yield return new WaitForSeconds(spawntime);
        
        while (true)
        {
            
            if (activeFoodCount < maxFoodOnScreen)
            {
                Spawn();
            }
            
            
            yield return foodWaitTime;
        }
    }

    private IEnumerator ObstacleSpawnRoutine()
    {
        
        yield return new WaitForSeconds(obstacleSpawnTime);
        
        while (true)
        {
            
            if (activeObstacleCount < maxObstaclesOnScreen)
            {
                SpawnFishHook();
            }
            
            
            yield return obstacleWaitTime;
        }
    }

    private void Spawn()
    {
        float laneY = laneManager.GetRandomLane();
        Vector3 spawnPos = new Vector3(spawnX, laneY, 0f);
        FoodData data = foodPrefab[UnityEngine.Random.Range(0, foodPrefab.Length)];

        activeFoodCount++;
        
        Events.RequestSpawn?.Invoke(data.prefab, spawnPos, Quaternion.identity, obj => 
        {
            FishFood food = obj.GetComponent<FishFood>();
            food.Init(data);
            food.SetReference(gManager);
            
            
            StartCoroutine(TrackFoodLifetime(obj));
        });
    }

    private void SpawnFishHook()
    {
        activeObstacleCount++;
        
        Events.RequestSpawn?.Invoke(obstacleData.prefab, obstacleData.SpawnPoint, Quaternion.identity,
            obj =>
            {
                obj.GetComponent<Obstacle>().Init(obstacleData, gManager);
                
                
                StartCoroutine(TrackObstacleLifetime(obj));
            }
        );
    }

    
    private IEnumerator TrackFoodLifetime(GameObject obj)
    {
        while (obj != null && obj.activeInHierarchy)
        {
            yield return null;
        }
        
        activeFoodCount = Mathf.Max(0, activeFoodCount - 1);
    }

    private IEnumerator TrackObstacleLifetime(GameObject obj)
    {
        while (obj != null && obj.activeInHierarchy)
        {
            yield return null;
        }
        
        activeObstacleCount = Mathf.Max(0, activeObstacleCount - 1);
    }

    // // Public methods to adjust spawn rates at runtime
    // public void SetFoodSpawnInterval(float interval)
    // {
    //     spawnInterval = interval;
    //     foodWaitTime = new WaitForSeconds(interval);
        
    //     // Restart coroutine with new interval
    //     if (foodSpawnCoroutine != null)
    //     {
    //         StopCoroutine(foodSpawnCoroutine);
    //         foodSpawnCoroutine = StartCoroutine(FoodSpawnRoutine());
    //     }
    // }

    // public void SetObstacleSpawnInterval(float interval)
    // {
    //     obstacleSpawnInterval = interval;
    //     obstacleWaitTime = new WaitForSeconds(interval);
        
    //     // Restart coroutine with new interval
    //     if (obstacleSpawnCoroutine != null)
    //     {
    //         StopCoroutine(obstacleSpawnCoroutine);
    //         obstacleSpawnCoroutine = StartCoroutine(ObstacleSpawnRoutine());
    //     }
    // }

    // public void SetMaxFoodOnScreen(int max)
    // {
    //     maxFoodOnScreen = max;
    // }

    // public void SetMaxObstaclesOnScreen(int max)
    // {
    //     maxObstaclesOnScreen = max;
    // }

    // // Debug info
    // public int GetActiveFoodCount() => activeFoodCount;
    // public int GetActiveObstacleCount() => activeObstacleCount;
}

    





