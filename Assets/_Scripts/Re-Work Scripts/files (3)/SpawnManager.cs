using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private LaneConfigSO laneConfig;
    [SerializeField] private FishDataSO   fishData;

    [Header("Spawn intervals (seconds)")]
    [SerializeField] private float fishIntervalStart = 2.0f;
    [SerializeField] private float fishIntervalMin   = 0.6f;
    [SerializeField] private float hookIntervalStart = 4.0f;
    [SerializeField] private float hookIntervalMin   = 1.5f;

    [Header("Difficulty ramp")]
    [SerializeField] private float difficultyRampTime = 20f;

    [Header("Camera for world bounds")]
    [SerializeField] private Camera gameCam;
    [Header("Spawn limits")]
    [SerializeField] private int maxFishPerLane = 2;


    private Dictionary <int,int> _fishCountPerLane = new();

    private ObjectPool _pool;
    private Coroutine  _fishRoutine;
    private Coroutine  _hookRoutine;
    private float _elapsed;
    private bool _running;

    void Awake()
    {
        _pool = ServiceLocator.Instance.Get<ObjectPool>();
    }

    public void StartSpawning()
    {
        _elapsed      = 0f;
        _running      = true;
        _fishCountPerLane.Clear();
        _fishRoutine  = StartCoroutine(SpawnFishLoop());
        _hookRoutine  = StartCoroutine(SpawnHookLoop());
    }

    public void StopSpawning()
    {
        _running = false;
        if (_fishRoutine != null) StopCoroutine(_fishRoutine);
        if (_hookRoutine != null) StopCoroutine(_hookRoutine);
    }

    public void ClearAll()
    {
        foreach (var fc in FindObjectsByType<FishController>(FindObjectsSortMode.None))
            if (fc.gameObject.activeSelf) fc.ReturnToPool();
        foreach (var hc in FindObjectsByType<HookController>(FindObjectsSortMode.None))
            if (hc.gameObject.activeSelf) hc.ReturnToPool();
    }

    private IEnumerator SpawnFishLoop()
    {
        var fishLanes = System.Array.FindAll(laneConfig.lanes,
        l => l.type == LaneConfigSO.LaneType.FishLeft || l.type == LaneConfigSO.LaneType.FishRight);

        for (int i = 0; i < fishLanes.Length; i++)
        {
            int laneIndex = System.Array.IndexOf(laneConfig.lanes, fishLanes[i]);
            _fishCountPerLane[laneIndex] = 0;
            float offset = (fishIntervalStart / fishLanes.Length) * i;
            StartCoroutine(SpawnFishLaneLoop(fishLanes[i], laneIndex, offset));
        }

        while (_running) yield return null;
    }

    private IEnumerator SpawnFishLaneLoop(LaneConfigSO.Lane lane, int laneIndex, float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay);
        while (_running)
        {
            if (_fishCountPerLane.GetValueOrDefault(laneIndex, 0) < maxFishPerLane)
            {
                SpawnFishOnLane(lane, laneIndex);
            }
            yield return new WaitForSeconds(CurrentFishInterval());
        }
    }

    private void SpawnFishOnLane(LaneConfigSO.Lane lane, int laneIndex)
    {
        int   direction = lane.type == LaneConfigSO.LaneType.FishLeft ? -1 : 1;
        int   sizeValue = WeightedRandomSize();
        float worldY    = NormYToWorld(lane.normalizedY);

        var obj = _pool.GetFish();
        var fc  = obj.GetComponent<FishController>();
        _fishCountPerLane[laneIndex]++;
        fc.Init(sizeValue, direction, worldY);

        // Decrement when fish exits/returns to pool
        fc.OnReturnedToPool += () =>
        {
            if (_fishCountPerLane.ContainsKey(laneIndex))
                _fishCountPerLane[laneIndex]--;
        };
    }



    private IEnumerator SpawnHookLoop()
    {
        yield return new WaitForSeconds(hookIntervalStart * 0.5f);
        while (_running)
        {
            SpawnHook();
            yield return new WaitForSeconds(CurrentHookInterval());
        }
    }

    private void SpawnFish()
    {
        var fishLanes = System.Array.FindAll(laneConfig.lanes,
            l => l.type == LaneConfigSO.LaneType.FishLeft ||
                 l.type == LaneConfigSO.LaneType.FishRight);

        if (fishLanes.Length == 0) return;

        var   lane      = fishLanes[Random.Range(0, fishLanes.Length)];
        int   direction = lane.type == LaneConfigSO.LaneType.FishLeft ? -1 : 1;
        int   sizeValue = WeightedRandomSize();
        float worldY    = NormYToWorld(lane.normalizedY);

        var obj = _pool.GetFish();
        obj.GetComponent<FishController>().Init(sizeValue, direction, worldY);
    }

    private void SpawnHook()
    {
        var hookLanes = System.Array.FindAll(laneConfig.lanes,l => l.type == LaneConfigSO.LaneType.HookDown);

        if (hookLanes.Length == 0) return;

        var lane = hookLanes[Random.Range(0, hookLanes.Length)];
        float worldX  = NormXToWorld(lane.normalizedX);
        float topY    = gameCam.orthographicSize + 1f;
        float bottomY = NormYToWorld(lane.hookMaxDepth);

        var obj = _pool.GetHook();
        obj.GetComponent<HookController>().Init(worldX, topY, bottomY);
    }

    private int WeightedRandomSize()
    {
        float t = Mathf.Clamp01(_elapsed / (difficultyRampTime * 2f));
        float roll = Random.value;
        if (roll < Mathf.Lerp(0.50f, 0.15f, t)) return 1;
        if (roll < Mathf.Lerp(0.80f, 0.40f, t)) return 2;
        if (roll < Mathf.Lerp(0.93f, 0.65f, t)) return 3;
        return 4;
    }

    private float CurrentFishInterval()
        => Mathf.Lerp(fishIntervalStart, fishIntervalMin, Mathf.Clamp01(_elapsed / difficultyRampTime));

    private float CurrentHookInterval()
        => Mathf.Lerp(hookIntervalStart, hookIntervalMin, Mathf.Clamp01(_elapsed / difficultyRampTime));

    private float NormYToWorld(float norm)
    {
        float h = gameCam.orthographicSize * 2f;
        return gameCam.transform.position.y - gameCam.orthographicSize + norm * h;
    }

    private float NormXToWorld(float norm)
    {
        float w = gameCam.orthographicSize * gameCam.aspect * 2f;
        return gameCam.transform.position.x - gameCam.orthographicSize * gameCam.aspect + norm * w;
    }

    void Update()
    {
        if (_running) _elapsed += Time.deltaTime;
    }




    #region Visual Lane Display

    #if UNITY_EDITOR
private void OnDrawGizmos()
{
    if (laneConfig == null || gameCam == null) return;

    foreach (var lane in laneConfig.lanes)
    {
        switch (lane.type)
        {
            case LaneConfigSO.LaneType.FishLeft:
            case LaneConfigSO.LaneType.FishRight:
                DrawFishLaneGizmo(lane);
                break;
            case LaneConfigSO.LaneType.HookDown:
                DrawHookLaneGizmo(lane);
                break;
        }
    }
}

private void DrawFishLaneGizmo(LaneConfigSO.Lane lane)
{
    bool isLeft = lane.type == LaneConfigSO.LaneType.FishLeft;
    Color color = isLeft ? new Color(0.2f, 0.6f, 1f, 0.85f) : new Color(0.2f, 1f, 0.6f, 0.85f);

    float y       = NormYToWorld(lane.normalizedY);
    float halfW   = gameCam.orthographicSize * gameCam.aspect;
    float leftX   = gameCam.transform.position.x - halfW;
    float rightX  = gameCam.transform.position.x + halfW;

    // Lane line
    Gizmos.color = color;
    Gizmos.DrawLine(new Vector3(leftX, y), new Vector3(rightX, y));

    // Direction arrow
    float arrowX     = isLeft ? rightX : leftX;
    float arrowDirX  = isLeft ? -1f : 1f;
    Vector3 origin   = new Vector3(arrowX, y);
    Vector3 tip      = origin + new Vector3(arrowDirX * 1.5f, 0f);
    Vector3 headUp   = origin + new Vector3(arrowDirX * 0.8f,  0.3f);
    Vector3 headDown = origin + new Vector3(arrowDirX * 0.8f, -0.3f);

    Gizmos.DrawLine(origin, tip);
    Gizmos.DrawLine(tip, headUp);
    Gizmos.DrawLine(tip, headDown);

    // Spawn point sphere
    Gizmos.DrawSphere(origin, 0.12f);

#if UNITY_EDITOR
    UnityEditor.Handles.color = color;
    UnityEditor.Handles.Label(
        new Vector3(arrowX + arrowDirX * 0.15f, y + 0.25f),
        lane.name
    );
#endif
}

private void DrawHookLaneGizmo(LaneConfigSO.Lane lane)
{
    Color color = new Color(1f, 0.45f, 0.2f, 0.85f);
    Gizmos.color = color;

    float x       = NormXToWorld(lane.normalizedX);
    float topY    = gameCam.transform.position.y + gameCam.orthographicSize + 1f;
    float bottomY = NormYToWorld(lane.hookMaxDepth);

    Vector3 top    = new Vector3(x, topY);
    Vector3 bottom = new Vector3(x, bottomY);

    // Drop line
    Gizmos.DrawLine(top, bottom);

    // Depth limit crossbar
    Gizmos.DrawLine(new Vector3(x - 0.3f, bottomY), new Vector3(x + 0.3f, bottomY));

    // Spawn sphere at top
    Gizmos.DrawSphere(top, 0.12f);

    // Arrow pointing down
    Vector3 arrowTip  = bottom + new Vector3(0f, 0.5f);
    Vector3 headLeft  = arrowTip + new Vector3(-0.25f, 0.4f);
    Vector3 headRight = arrowTip + new Vector3( 0.25f, 0.4f);

    Gizmos.DrawLine(arrowTip, headLeft);
    Gizmos.DrawLine(arrowTip, headRight);

#if UNITY_EDITOR
    UnityEditor.Handles.color = color;
    UnityEditor.Handles.Label(new Vector3(x + 0.15f, topY - 0.3f), lane.name);
    UnityEditor.Handles.Label(new Vector3(x + 0.15f, bottomY - 0.3f), $"depth {lane.hookMaxDepth:F2}");
#endif
}
#endif

#endregion
}
