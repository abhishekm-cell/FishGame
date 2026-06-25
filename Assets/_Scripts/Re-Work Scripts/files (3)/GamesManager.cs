using UnityEngine;

/// <summary>
/// Controls game state and score. Registered into ServiceLocator by GameBootstrapper.
/// Receives SpawnManager and PlayerController via Inject().
/// </summary>
public class GamesManager : MonoBehaviour
{
    public enum GameState { MainMenu, Playing, GameOver }
    public GameState State { get; private set; } = GameState.MainMenu;
    public int Score { get; private set; }

    // Injected via GameBootstrapper → Inject()
    private SpawnManager _spawnManager;
    private PlayerController _player;

    /// <summary>Called by GameBootstrapper after all services are registered.</summary>
    public void Inject(SpawnManager spawnManager, PlayerController player)
    {
        _spawnManager = spawnManager;
        _player = player;
    }

    void OnEnable()
    {
        EventBus.Subscribe<OnPlayerDied>(HandlePlayerDied);
        //EventBus.Subscribe<OnHookCaught>(HandleHookCaught);
        EventBus.Subscribe<OnPlayerAte>(HandlePlayerAte);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<OnPlayerDied>(HandlePlayerDied);
        //EventBus.Unsubscribe<OnHookCaught>(HandleHookCaught);
        EventBus.Unsubscribe<OnPlayerAte>(HandlePlayerAte);
    }

    public void StartGame()
    {
        Score = 0;
        State = GameState.Playing;
        _player.ResetPlayer();
        _spawnManager.StartSpawning();
        EventBus.Publish(new OnGameStarted());
        EventBus.Publish(new OnScoreChanged { score = Score });
    }

    public void RestartGame()
    {
        _spawnManager.StopSpawning();
        _spawnManager.ClearAll();
        StartGame();
    }

    private void HandlePlayerAte(OnPlayerAte evt)
    {
        Score += evt.eatenFishSize * 10;
        EventBus.Publish(new OnScoreChanged { score = Score });
    }

    private void HandlePlayerDied(OnPlayerDied evt) => TriggerGameOver(evt.reason);
    //private void HandleHookCaught(OnHookCaught _)   => TriggerGameOver("hook");

    private void TriggerGameOver(string reason)
    {
        if (State != GameState.Playing) return;
        State = GameState.GameOver;
        _spawnManager.StopSpawning();
        EventBus.Publish(new OnGameOver { reason = reason, finalScore = Score });
    }
}
