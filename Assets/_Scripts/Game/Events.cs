using System;
using UnityEngine;

public static class Events 
{
    // for pooling
    public static Action<GameObject, Vector3, Quaternion, Action <GameObject>> RequestSpawn;
    public static Action<GameObject, GameObject> RequestDespawn;

    // UI Handlers
    public static event Action MainMenuPanelShow;
    public static void ShowMainMenuInvoke() => MainMenuPanelShow?.Invoke();

    public static event Action InGamePanelShow;
    public static void ShowInGameInvoke() => InGamePanelShow?.Invoke();

    public static event Action GameOverPanelShow;
    public static void ShowGameOverInvoke() => GameOverPanelShow?.Invoke();

    public static event Action GamePausePanelShow;
    public static void ShowGamePauseInvoke() => GamePausePanelShow?.Invoke();

    
    // for points
    public static event Action<int> ScoreGained;
    public static void ScoreGainedInvoke(int points) => ScoreGained?.Invoke(points);
    public static event Action <int> UpdateScore;
    public static void UpdateScoreInvoke(int amount) => UpdateScore?.Invoke(amount);

    // for GameState 
    public static event Action PauseRequested;
    public static event Action ResumeRequested;
    public static event Action GameOverRequested;
    public static event Action GameStartRequested;
    public static event Action ResetGameRequested;

    public static void PauseRequest() => PauseRequested?.Invoke();
    public static void ResumeRequest() => ResumeRequested?.Invoke();
    public static void GameOverRequest() => GameOverRequested?.Invoke();
    public static void GameStartRequest() => GameStartRequested?.Invoke();
    public static void ResetGameRequest() => ResetGameRequested?.Invoke();


    public static event Action GameStart;
    public static void GameStartInvoke() => GameStart?.Invoke();

    public static event Action ResetGame;
    public static void ResetGameInvoke() => ResetGame?.Invoke();

    // MY NEW EVETNS :)

    public static event Action GameInit;
    public static void GameInitInvoke() => GameInit?.Invoke();

    public static event Action GameStarted;
    public static void GameStartedInvoke() => GameStarted?.Invoke();

    public static event Action GamePaused;
    public static void GamePausedInvoke() => GamePaused?.Invoke();


    
}