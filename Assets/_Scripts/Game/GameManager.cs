using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ObjectPoolManager objectPoolManager;
    [SerializeField] private SpawnSystem spawnSystem;
    [SerializeField] private LaneManager laneManager;
    [SerializeField] private Movement playermove;
    //[SerializeField] private FishFood fishFood;
    [SerializeField] private PlayerGrowth playerGrowth;
    //[SerializeField] private bool baitEaten = false;
    // managers
    public ObjectPoolManager GetObjectPoolManager() => objectPoolManager; 
    public SpawnSystem GetSpawnSystem() => spawnSystem;
    public LaneManager GetLaneManager() => laneManager;
    public Movement GetPlayerMovement() => playermove;
    //public FishFood GetFishFood() => fishFood;
    public PlayerGrowth GetPlayerGrowth() => playerGrowth;
    void Awake()
    {
        //fishFood.SetReference(this);
        playerGrowth.SetReferences(this);
    }
}