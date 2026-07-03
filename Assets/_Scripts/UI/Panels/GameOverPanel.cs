using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button replayButton;
    [SerializeField] private Button homeButton;
 
    void OnEnable()
    {
        ButtonListeners();
        //AudioManager.Instance.PlaySFX(SoundType.Dead);        
    }
 
    void OnDisable()
    {
        replayButton.onClick.RemoveListener(OnReplayClicked);
        homeButton.onClick.RemoveListener(OnHomeClicked);
    }
 
    private void ButtonListeners()
    {
        replayButton.onClick.AddListener(OnReplayClicked);
        homeButton.onClick.AddListener(OnHomeClicked);
    }
 
    void OnReplayClicked()
    {
        Debug.Log("Replay Button Clicked");
        AudioManager.Instance.PlaySFX(SoundType.Button);
        ServiceLocator.Instance.Get<GamesManager>().RestartGame();
    }
    void OnHomeClicked()
    {
        Debug.Log("Home Button Clicked");
        AudioManager.Instance.PlaySFX(SoundType.Button);
        ServiceLocator.Instance.Get<GamesManager>().ReturnToMainMenu();
    }
 
 
    public void Show()
    {
        gameObject.SetActive(true);
        
    }
 
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}