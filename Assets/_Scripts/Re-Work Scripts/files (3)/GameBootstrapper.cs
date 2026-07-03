using UnityEngine;

/// <summary>
/// Single entry point. Runs at execution order -100 (before all other Awake calls).
///
/// Wiring order:
///   1. Register all services into ServiceLocator.
///   2. Inject cross-service dependencies that can't go through the locator
///      (e.g. GameManager needs SpawnManager + PlayerController by direct ref,
///       not via locator, because they're not services themselves).
/// </summary>
[DefaultExecutionOrder(-100)]
public class GameBootstrapper : MonoBehaviour
{
    [Header("Services to register")]
    [SerializeField] private GamesManager gameManager;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private UIManagers uiManager;

    [Header("Non-service dependencies for GameManager")]
    [SerializeField] private PlayerController player;

    void Awake()
    {
        var sl = GetComponent<ServiceLocator>();
        if (sl == null)
        {
            Debug.LogError("[GameBootstrapper] ServiceLocator component missing.");
            return;
        }

        
        sl.Register<GamesManager>(gameManager);
        sl.Register<SpawnManager>(spawnManager);
        sl.Register<ObjectPool>(objectPool);
        sl.Register<UIManagers>(uiManager);

        
        gameManager.Inject(spawnManager, player);
    }
}
