using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "FishBait/Fish Data")]
public class FishDataSO : ScriptableObject
{
    [System.Serializable]
    public class SizeTier
    {
        public int sizeValue;      
        public float speed;
        public float colliderRadius;
        public Vector2 colliderOffset;
        public Sprite sprite;
        [Tooltip("Visual scale applied to the sprite renderer")]
        public float visualScale = 1f;
        [Tooltip("Prefab for this tier fish(using self animator). will get used by ObjectPool for polling")]
        public GameObject preFab;
    }

    public SizeTier[] tiers;
    public int TierCount => tiers[tiers.Length - 1].sizeValue;

    public SizeTier GetTier(int sizeValue)
    {
        foreach (var t in tiers)
        {
            if (t.sizeValue == sizeValue) return t;
        }
            
        Debug.LogWarning($"[FishDataSO] No tier for sizeValue={sizeValue}, using tiers[0]");
        return tiers[0];
    }
}
