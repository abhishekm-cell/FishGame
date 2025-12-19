using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LaneData", menuName = "ScriptableObjects/LaneData")]
public class LaneDataSO : ScriptableObject
{
    public int laneCount = 7;
    public float verticalOffset = 2.5f;
    public float centreY = 0f;
   

   public float[] GetLaneYPositions()
    {
        float[] laneYPositions = new float[laneCount];
        float startY = centreY - ((laneCount - 1) * verticalOffset) / 2f;

        for (int i = 0; i < laneCount; i++)
        {
            laneYPositions[i] = startY + verticalOffset * i;
        }
        return laneYPositions;
    }
}
