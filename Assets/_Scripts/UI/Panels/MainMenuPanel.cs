using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button quitButton;

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
        startButton.onClick.AddListener(StartButtonClicked);
        infoButton.onClick.AddListener(InfoButtonClicked);
        quitButton.onClick.AddListener(QuitButtonClicked);
    }
    
    void StartButtonClicked()//playButton
    {
        Debug.Log("Start Button Clicked");
        Events.GameStartedInvoke();
        // Events.ResetGameRequest();
        // Events.GameStartInvoke();
        // Events.ShowInGameInvoke();
    }

    void InfoButtonClicked()
    {
        Debug.Log("Info Button Clicked");
    }

    void QuitButtonClicked()
    {
        Application.Quit();
        Debug.Log("Quit Button Clicked");
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