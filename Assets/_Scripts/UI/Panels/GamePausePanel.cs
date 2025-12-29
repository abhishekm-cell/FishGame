using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePausePanel : MonoBehaviour
{   
    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button homeButton;// back to main menu

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
       resumeButton.onClick.AddListener(OnResumeClicked);
       homeButton.onClick.AddListener(OnHomeClicked); 
    }


    void OnResumeClicked()
    {
        Debug.Log("Resume Button Clicked");
        Events.ShowInGameInvoke();
        
        Events.ResumeRequest();
        
    }

    void OnHomeClicked()
    {
        Debug.Log("Home Button Clicked");
        Events.ShowMainMenuInvoke();
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