using UnityEngine;

// Attach one of these to a LaneConfig asset.
// Create four fish lanes and two hook lanes in the Inspector.
[CreateAssetMenu(fileName = "LaneConfig", menuName = "FishBait/Lane Config")]
public class LaneConfigSO : ScriptableObject
{
    public enum LaneType { FishLeft, FishRight, HookDown }

    [System.Serializable]
    public class Lane
    {
        public string   name;
        public LaneType type;

        [Tooltip("Normalised Y position (0=bottom, 1=top) for fish lanes")]
        [Range(0f, 1f)]
        public float    normalizedY;

        [Tooltip("Normalised X position (0=left, 1=right) for hook lanes")]
        [Range(0f, 1f)]
        public float    normalizedX;

        [Tooltip("How deep (normalised) hooks travel before retracting")]
        [Range(0f, 1f)]
        public float    hookMaxDepth = 0.85f;
    }

    public Lane[] lanes;
}
