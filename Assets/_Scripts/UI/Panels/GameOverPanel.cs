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
    }

    void OnDisable()
    {
        ButtonListeners();
    }

    private void ButtonListeners()
    {
        replayButton.onClick.AddListener(OnReplayClicked);
        homeButton.onClick.AddListener(OnHomeClicked);
    }

    void OnReplayClicked()
    {
        Debug.Log("Replay Button Clicked");
        Events.GameStartInvoke();
    }
    void OnHomeClicked()
    {
        Debug.Log("Home Button Clicked");
        Events.ResetGameRequest();
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