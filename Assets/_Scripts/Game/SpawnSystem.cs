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
 
    [Header("Difficulty Scaling")]
    [Tooltip("Leave empty to auto-generate a ramp from the baseline Spawn Settings/Spawn Limits above. " +
             "Fill this in to hand-tune each tier instead.")]
    [SerializeField] private DifficultyTier[] difficultyTiers;
    
    [Header("References")]
    [SerializeField] private GameManager gManager;
 
    // Coroutine references for control
    private Coroutine foodSpawnCoroutine;
    private Coroutine obstacleSpawnCoroutine;
    
    // Track active spawns
    private int activeFoodCount = 0;
    private int activeObstacleCount = 0;
 
    // Difficulty state
    private DifficultyTier[] activeTiers;
    private float gameStartTime;
 
    // Cached for weighted food selection
    private float maxFoodPoints;
 
    [Serializable]
    public struct DifficultyTier
    {
        [Tooltip("Seconds since GameStart at which this tier kicks in")]
        public float timeThreshold;
        public float foodSpawnInterval;
        public float obstacleSpawnInterval;
        public int maxFoodOnScreen;
        public int maxObstaclesOnScreen;
        [Tooltip("Multiplies obstacle bob frequency and horizontal speed")]
        public float obstacleMultiplier;
    }
 
    private struct DifficultyState
    {
        public DifficultyTier tier;
        public float progress01; // 0 = run start, 1 = hardest tier reached
    }
 
    void Awake()
    {
        maxFoodPoints = 0f;
        foreach (var f in foodPrefab)
        {
            if (f != null && f.points > maxFoodPoints)
                maxFoodPoints = f.points;
        }
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
 
        gameStartTime = Time.time;
        activeTiers = GetEffectiveTiers();
        
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
 
    // Returns the designer-authored tiers if any were set in the Inspector,
    // otherwise builds a default 4-stage ramp off the baseline fields above
    // so the system works even with the array left empty.
    private DifficultyTier[] GetEffectiveTiers()
    {
        if (difficultyTiers != null && difficultyTiers.Length > 0)
            return difficultyTiers;
 
        return new DifficultyTier[]
        {
            new DifficultyTier
            {
                timeThreshold = 0f,
                foodSpawnInterval = spawnInterval,
                obstacleSpawnInterval = obstacleSpawnInterval,
                maxFoodOnScreen = maxFoodOnScreen,
                maxObstaclesOnScreen = maxObstaclesOnScreen,
                obstacleMultiplier = 1.0f
            },
            new DifficultyTier
            {
                timeThreshold = 20f,
                foodSpawnInterval = spawnInterval * 0.75f,
                obstacleSpawnInterval = obstacleSpawnInterval * 0.8f,
                maxFoodOnScreen = maxFoodOnScreen + 2,
                maxObstaclesOnScreen = maxObstaclesOnScreen + 1,
                obstacleMultiplier = 1.15f
            },
            new DifficultyTier
            {
                timeThreshold = 45f,
                foodSpawnInterval = spawnInterval * 0.55f,
                obstacleSpawnInterval = obstacleSpawnInterval * 0.6f,
                maxFoodOnScreen = maxFoodOnScreen + 4,
                maxObstaclesOnScreen = maxObstaclesOnScreen + 2,
                obstacleMultiplier = 1.3f
            },
            new DifficultyTier
            {
                timeThreshold = 75f,
                foodSpawnInterval = spawnInterval * 0.4f,
                obstacleSpawnInterval = obstacleSpawnInterval * 0.44f,
                maxFoodOnScreen = maxFoodOnScreen + 6,
                maxObstaclesOnScreen = maxObstaclesOnScreen + 3,
                obstacleMultiplier = 1.5f
            },
        };
    }
 
    private DifficultyState GetDifficultyState()
    {
        float elapsed = Time.time - gameStartTime;
 
        DifficultyTier tier = activeTiers[0];
        for (int i = 0; i < activeTiers.Length; i++)
        {
            if (elapsed >= activeTiers[i].timeThreshold)
                tier = activeTiers[i];
        }
 
        float maxThreshold = activeTiers[activeTiers.Length - 1].timeThreshold;
        float progress = maxThreshold > 0f ? Mathf.Clamp01(elapsed / maxThreshold) : 1f;
 
        return new DifficultyState { tier = tier, progress01 = progress };
    }
 
    private IEnumerator FoodSpawnRoutine()
    {
        yield return new WaitForSeconds(spawntime);
        
        while (true)
        {
            DifficultyState diff = GetDifficultyState();
 
            if (activeFoodCount < diff.tier.maxFoodOnScreen)
            {
                Spawn(diff);
            }
            
            yield return new WaitForSeconds(diff.tier.foodSpawnInterval);
        }
    }
 
    private IEnumerator ObstacleSpawnRoutine()
    {
        yield return new WaitForSeconds(obstacleSpawnTime);
        
        while (true)
        {
            DifficultyState diff = GetDifficultyState();
 
            if (activeObstacleCount < diff.tier.maxObstaclesOnScreen)
            {
                SpawnFishHook(diff.tier.obstacleMultiplier);
            }
            
            yield return new WaitForSeconds(diff.tier.obstacleSpawnInterval);
        }
    }
 
    private void Spawn(DifficultyState diff)
    {
        float laneY = laneManager.GetRandomLane();
        Vector3 spawnPos = new Vector3(spawnX, laneY, 0f);
        FoodData data = GetWeightedFood(diff.progress01);
 
        activeFoodCount++;
        
        Events.RequestSpawn?.Invoke(data.prefab, spawnPos, Quaternion.identity, obj => 
        {
            FishFood food = obj.GetComponent<FishFood>();
            food.Init(data);
            food.SetReference(gManager);
            
            
            StartCoroutine(TrackFoodLifetime(obj));
        });
    }
 
    // Early-game: biased toward low-point (small) fish so the player isn't
    // ambushed before they've grown. Late-game: biased toward high-point
    // (large/worm) fish so risk keeps escalating. Nothing ever hits zero
    // odds, so variety never fully disappears at either end.
    private FoodData GetWeightedFood(float progress01)
    {
        float totalWeight = 0f;
        float[] weights = new float[foodPrefab.Length];
 
        for (int i = 0; i < foodPrefab.Length; i++)
        {
            float sizeBias = maxFoodPoints > 0f ? Mathf.Clamp01(foodPrefab[i].points / maxFoodPoints) : 0.5f;
            float weight = Mathf.Lerp(1f - sizeBias, sizeBias, progress01) + 0.1f;
            weights[i] = weight;
            totalWeight += weight;
        }
 
        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;
 
        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative)
                return foodPrefab[i];
        }
 
        return foodPrefab[foodPrefab.Length - 1];
    }
 
    private void SpawnFishHook(float difficultyMultiplier)
    {
        activeObstacleCount++;
        
        Events.RequestSpawn?.Invoke(obstacleData.prefab, obstacleData.SpawnPoint, Quaternion.identity,
            obj =>
            {
                obj.GetComponent<Obstacle>().Init(obstacleData, gManager, difficultyMultiplier);
                
                
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

    
}

    





