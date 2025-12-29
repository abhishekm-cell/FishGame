using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private SpawnSystem spawnSystem;
    [SerializeField] private ObjectPoolManager poolManager;
    [SerializeField] private Movement playerMovement;
    [SerializeField] private PlayerGrowth playerGrowth;
    [SerializeField] private UIManager uiManager;

    private GameStates currentState;
    private int score;

    void Awake()
    {
        uiManager.Init(this);
        
    }

    void OnEnable()
    {
        Events.GameInit += GameInit;
        Events.GameStarted += StartGame;
        Events.GamePaused += PauseGame;

        // Events.PauseRequested += PauseGame;
        Events.ResumeRequested += ResumeGame;
        Events.GameOverRequested += TriggerGameOver;
        Events.ResetGameRequested += ResetGame;
        Events.ScoreGained += AddScore;
    }

    void OnDisable()
    {
        Events.GameInit -= GameInit;
        Events.GameStarted -= StartGame;
        Events.GamePaused -= PauseGame;

        // Events.PauseRequested -= PauseGame;
        Events.ResumeRequested -= ResumeGame;
        Events.GameOverRequested -= TriggerGameOver;
        Events.ResetGameRequested -= ResetGame;
        Events.ScoreGained -= AddScore;
    }

    private void SetState(GameStates newState)
    {
        currentState = newState;
        Debug.Log("STATE → " + newState);

        switch (newState)
        {
            case GameStates.MainMenu:
                // Time.timeScale = 1f;
                break;

            case GameStates.InGame:
                // Time.timeScale = 1f;
                break;

            case GameStates.GamePause:
                // Time.timeScale = 0f;
                break;

            case GameStates.GameOver:
                // Time.timeScale = 0f;
                break;
        }
    }

    private void GameInit()
    {
        SetState(GameStates.MainMenu);
        // spawnSystem.StopSpawning();
        // poolManager.DeSpawnAll();
        // playerMovement.ResetPlayer();
        // playerGrowth.ResetGrowth();
    }
    
    private void StartGame()
    {
        score = 0;
        Events.UpdateScoreInvoke(score);
        SetState(GameStates.InGame);
    }

    private void PauseGame()
    {
        if (currentState == GameStates.InGame)
        {
            SetState(GameStates.GamePause);
        }
            
    }

    private void ResumeGame()
    {
        if (currentState == GameStates.GamePause)
        {
            SetState(GameStates.InGame);
        }
            
    }

    public void TriggerGameOver()
    {
        SetState(GameStates.GameOver);
    }

    private void ResetGame()
    {
        score = 0;
        Events.UpdateScoreInvoke(score);

        // spawnSystem.StopSpawning();
        // poolManager.DeSpawnAll();

        playerMovement.ResetPlayer();
        playerGrowth.ResetGrowth();
    }

    private void AddScore(int amount)
    {
        score += amount;
        Events.UpdateScoreInvoke(score);
    }


    public PlayerGrowth GetPlayerGrowth() => playerGrowth;
    public Movement GetPlayerMovement() => playerMovement;
}



