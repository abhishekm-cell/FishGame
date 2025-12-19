using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private LaneManager laneManager;
    [SerializeField] private GameObject[] foodPrefab;

    [SerializeField] private float spawnX = 10f;
    [SerializeField] private float spawnInterval = 2f;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, spawnInterval);
    }

    // Update is called once per frame
    
    void Spawn()
    {
        float laneY = laneManager.GetRandomLane();

        Vector3 spawnPos = new Vector3(spawnX, laneY, 0f);
        Instantiate(foodPrefab[Random.Range(0, foodPrefab.Length)],spawnPos,Quaternion.identity);
        
    }
}
