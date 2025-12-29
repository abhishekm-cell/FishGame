using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private MainMenuPanel mainMenu;
    [SerializeField] private InGamePanel inGame;
    [SerializeField] private GameOverPanel gameOver;
    [SerializeField] private GamePausePanel pause;
    //[SerializeField] private GameManager gManager;

    public void Init(GameManager gm)
    {
        //gManager = gm;
        ShowMainMenu();
        Events.GameInitInvoke();
    }

    void OnEnable()
    {
        Events.GameInit += ShowMainMenu;
        Events.GameStarted += ShowInGame;
        Events.GamePaused += ShowPause; 
    }

    void OnDisable()
    {
        Events.GameInit -= ShowMainMenu;
        Events.GameStarted -= ShowInGame;
        Events.GamePaused -= ShowPause;
    }

    private void ShowMainMenu()
    {
        mainMenu.Show();
        inGame.Hide();
        gameOver.Hide();
        pause.Hide();
        
    }

    private void ShowInGame()
    {
        mainMenu.Hide();
        inGame.Show();
        gameOver.Hide();
        pause.Hide();
    }

    private void ShowGameOver()
    {
        mainMenu.Hide();
        inGame.Hide();
        gameOver.Show();
        pause.Hide();
    }

    private void ShowPause()
    {
        pause.Show();
    }
}    





