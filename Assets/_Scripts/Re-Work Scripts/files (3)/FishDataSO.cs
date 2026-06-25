using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "FishBait/Fish Data")]
public class FishDataSO : ScriptableObject
{
    [System.Serializable]
    public class SizeTier
    {
        public int sizeValue;      // 1 = tiny … 5 = huge
        public float speed;
        public float colliderRadius;
        public Sprite   sprite;
        [Tooltip("Visual scale applied to the sprite renderer")]
        public float visualScale = 1f;
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
