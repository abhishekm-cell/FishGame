using UnityEngine;
using TMPro;
using System;

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
    private int highScore;


    void Awake()
    {
        //uiManager.Init(this);
        SetState(GameStates.MainMenu);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScore();
    }

    void OnEnable()
    {
        Events.GameStart += StartGame;
        Events.PauseRequested += PauseGame;
        Events.ResumeRequested += ResumeGame;
        //Events.GameOverRequested += TriggerGameOver;
        Events.ResetGameRequested += ResetGame;
        Events.ScoreGained += AddScore;
    }

    void OnDisable()
    {
        Events.GameStart -= StartGame;
        Events.PauseRequested -= PauseGame;
        Events.ResumeRequested -= ResumeGame;
        //Events.GameOverRequested -= TriggerGameOver;
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
                Time.timeScale = 1f;
                uiManager.ShowMainMenu();
                break;

            case GameStates.InGame:
                Time.timeScale = 1f;
                uiManager.ShowInGame();
                break;

            case GameStates.GamePause:
                Time.timeScale = 0f;
                uiManager.ShowPause();
                break;

            case GameStates.GameOver:
                Time.timeScale = 0f;
                uiManager.ShowGameOver();
                break;
        }
    }

    
    private void StartGame()
    {
        ResetGame();
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
        Events.GameOverRequest();
    }

    private void ResetGame()
    {
        UpdateHighScore();
        score = 0;
        Events.UpdateScoreInvoke(score);

        spawnSystem.StopSpawning();
        poolManager.DeSpawnAll();

        playerMovement.ResetPlayer();
        playerGrowth.ResetGrowth();
        
        Debug.Log("Game Reset");
    }

    private void AddScore(int amount)
    {
        score += amount;
        Events.UpdateScoreInvoke(score);
    }

    private void UpdateHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }


    public PlayerGrowth GetPlayerGrowth() => playerGrowth;
    public Movement GetPlayerMovement() => playerMovement;
    
}



