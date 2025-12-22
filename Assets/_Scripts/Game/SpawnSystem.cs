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
    [SerializeField] private float spawntime;
    [SerializeField] private GameManager gManager;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(Spawn), spawntime, spawnInterval);
    }

    // Update is called once per frame
    
    void Spawn()
    {
        float laneY = laneManager.GetRandomLane();

        Vector3 spawnPos = new Vector3(spawnX, laneY, 0f);
        FoodData data = foodPrefab[Random.Range(0, foodPrefab.Length)];


        Events.RequestSpawn?.Invoke(data.prefab, spawnPos, Quaternion.identity, obj => {obj.GetComponent<FishFood>().Init(data, gManager);});

        
        
    }
}
