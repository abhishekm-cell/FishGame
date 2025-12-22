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
    [SerializeField] private float spawnInterval = 2f, obstacleSpawnInterval,obstacleSpawnTime;
    [SerializeField] private float spawntime;
    [SerializeField] private GameManager gManager;

    [SerializeField] private ObstacleData obstacleData;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(Spawn), spawntime, spawnInterval);
        InvokeRepeating(nameof(SpawnFishHook), obstacleSpawnTime, obstacleSpawnInterval);
    }

    // Update is called once per frame
    
    private void Spawn()
    {
        float laneY = laneManager.GetRandomLane();

        Vector3 spawnPos = new Vector3(spawnX, laneY, 0f);
        FoodData data = foodPrefab[UnityEngine.Random.Range(0, foodPrefab.Length)];


        Events.RequestSpawn?.Invoke(data.prefab, spawnPos, Quaternion.identity, obj => {obj.GetComponent<FishFood>().Init(data, gManager);});
        
    }

    private void SpawnFishHook()
    {
        Debug.Log("Spawning Obstacle");
        Events.RequestSpawn?.Invoke(obstacleData.prefab,obstacleData.SpawnPoint,Quaternion.identity,
            (obj) =>
            {
                obj.GetComponent<Obstacle>().Init(obstacleData, gManager);
            }
        );   
    }




}
