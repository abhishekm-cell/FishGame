using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public LaneDataSO laneData;
    public float[] lanes {get; private set;}

    void Awake()
    {
        lanes = laneData.GetLaneYPositions();
    }

    public float GetRandomLane()
    {
        return lanes[Random.Range(0, lanes.Length)];
    }

    public float GetLaneByIndex(int index)
    {
        return lanes[Mathf.Clamp(index, 0, lanes.Length - 1)];
    }
}