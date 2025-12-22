using UnityEngine;


public class LaneManager : MonoBehaviour
{
    [SerializeField] private Transform laneParent;
    public float[] lanes { get; private set; }

    void Awake()
    {
        CacheLanes();
    }

    void CacheLanes()
    {
        lanes = new float[laneParent.childCount];

        for (int i = 0; i < laneParent.childCount; i++)
        {
            lanes[i] = laneParent.GetChild(i).position.y;
        }
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

