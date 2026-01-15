using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private MainMenuPanel mainMenu;
    [SerializeField] private InGamePanel inGame;
    [SerializeField] private GameOverPanel gameOver;
    [SerializeField] private GamePausePanel pause;
    [SerializeField] private InfoPanel info;
    //[SerializeField] private GameManager gManager;

    void OnEnable()
    {
        Events.MainMenuPanelShow += ShowMainMenu;
        Events.UpdateScore += UpdateScoreInPanel;
        Events.InfoPanelRequested += ShowInfoPanel;

    }

    

    void OnDisable()
    {
        Events.MainMenuPanelShow -= ShowMainMenu;
        Events.UpdateScore -= UpdateScoreInPanel;
        Events.InfoPanelRequested -= ShowInfoPanel;
    }

    public void Init(GameManager gm)
    {
        //gManager = gm;
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenu.Show();
        inGame.Hide();
        gameOver.Hide();
        pause.Hide();
        info.Hide();
        
    }

    public void ShowInGame()
    {
        mainMenu.Hide();
        inGame.Show();
        gameOver.Hide();
        pause.Hide();
        info.Hide();

    }

    public void ShowGameOver()
    {
        mainMenu.Hide();
        inGame.Show();
        gameOver.Show();
        pause.Hide();
        info.Hide();
    }

    public void ShowPause()
    {
        pause.Show();
    }
    public void ShowInfoPanel()
    {
        info.Show();
    }
    private void UpdateScoreInPanel(int CurrentScore)
    {
        pause.UpdateCurrentScore(CurrentScore);
    }
}    





