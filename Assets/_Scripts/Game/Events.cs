using System;
using UnityEngine;

public static class Events 
{
    // for pooling
    public static Action<GameObject, Vector3, Quaternion, Action <GameObject>> RequestSpawn;
    public static Action<GameObject, GameObject> RequestDespawn;

    // for points
    
}