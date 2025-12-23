using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ObjectPoolManager objectPoolManager;
    [SerializeField] private SpawnSystem spawnSystem;
    [SerializeField] private LaneManager laneManager;
    [SerializeField] private Movement playermove;
    // managers
    public ObjectPoolManager GetObjectPoolManager() => objectPoolManager; 
    public SpawnSystem GetSpawnSystem() => spawnSystem;
    public LaneManager GetLaneManager() => laneManager;
    public Movement GetPlayerMovement() => playermove;
    void Awake()
    {
        
    }
}