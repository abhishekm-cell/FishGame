using System.Collections;
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

    private ObjectPool _pool;
    private Coroutine  _fishRoutine;
    private Coroutine  _hookRoutine;
    private float      _elapsed;
    private bool       _running;

    void Awake()
    {
        _pool = ServiceLocator.Instance.Get<ObjectPool>();
    }

    public void StartSpawning()
    {
        _elapsed      = 0f;
        _running      = true;
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
        while (_running)
        {
            SpawnFish();
            yield return new WaitForSeconds(CurrentFishInterval());
        }
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
        var hookLanes = System.Array.FindAll(laneConfig.lanes,
            l => l.type == LaneConfigSO.LaneType.HookDown);

        if (hookLanes.Length == 0) return;

        var   lane    = hookLanes[Random.Range(0, hookLanes.Length)];
        float worldX  = NormXToWorld(lane.normalizedX);
        float topY    = gameCam.orthographicSize + 1f;
        float bottomY = NormYToWorld(lane.hookMaxDepth);

        var obj = _pool.GetHook();
        obj.GetComponent<HookController>().Init(worldX, topY, bottomY);
    }

    private int WeightedRandomSize()
    {
        float t    = Mathf.Clamp01(_elapsed / (difficultyRampTime * 2f));
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
}
