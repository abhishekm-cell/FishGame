using UnityEngine;

[CreateAssetMenu(menuName = "FishGame/PlayerFoodGrowthData")]
public class PlayerFoodGrowthData : ScriptableObject
{
    public GrowthStage stage;
    public Vector3 targetScale;

    [Header("Growth Requirements")]
    public int smallFish;
    public int mediumFish ;
    public int largeFish;

    [Header("Allowed Limit")]

    public int maxAllowedLimit;    
}