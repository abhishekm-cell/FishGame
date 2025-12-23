using UnityEngine;

[CreateAssetMenu(menuName = "FishGame/ObstacleData")]
public class ObstacleData : ScriptableObject
{
    public GameObject prefab;
    public Vector2 SpawnPoint;
    public float maxAmplitude; // length of Y movement // How far up and down
    public float maxFrequency; //  up/down speed for ping pong  // How fast it moves
    public float minAmplitude;
    public float minFrequency;

    public float horiSpeed; // movespeed horizontal
    public float reelSpeed;

}