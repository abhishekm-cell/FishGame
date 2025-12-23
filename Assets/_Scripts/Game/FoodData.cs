using UnityEngine;

[CreateAssetMenu(menuName = "FishGame/FoodData")]
public class FoodData : ScriptableObject
{
    [Header("Points")]
    public int points;
    [Header("Prefab")]
    public GameObject prefab;
    public float deathDelay = 0.5f;
    
    [Header("Movment")]
    public float moveSpeed;

    [Header("Effects")]
    public FoodEffect foodEffect;

    public float effectStrength = 0.5f;
    public float effectDuration = 2f;

}