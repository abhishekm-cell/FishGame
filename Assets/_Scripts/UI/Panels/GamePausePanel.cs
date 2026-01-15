using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePausePanel : MonoBehaviour
{   
    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button homeButton;// back to main menu
    [Header("Score UI")]    
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    

    void OnEnable()
    {
        ButtonListeners();
        Events.UpdateScore += UpdateCurrentScore;
        UpdateHighScore();
    }

    void OnDisable()
    {
        resumeButton.onClick.RemoveListener(OnResumeClicked);
       homeButton.onClick.RemoveListener(OnHomeClicked);
        Events.UpdateScore -= UpdateCurrentScore;
        
    }

    private void ButtonListeners()
    {
       resumeButton.onClick.AddListener(OnResumeClicked);
       homeButton.onClick.AddListener(OnHomeClicked); 
    }


    void OnResumeClicked()
    {
        Debug.Log("Resume Button Clicked");
        AudioManager.Instance.PlaySFX(SoundType.Button);
        AudioManager.Instance.ResumeSFX();
        Events.ShowInGameInvoke();
        
        Events.ResumeRequest();
        
    }

    void OnHomeClicked()
    {
        Debug.Log("Home Button Clicked");
        AudioManager.Instance.PlaySFX(SoundType.Button);
        Events.ResetGameRequest();
        Events.ShowMainMenuInvoke();
    }  

    public void UpdateCurrentScore(int score)
    {
        currentScoreText.text = $"Score: {score} ";
    }

    public void UpdateHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore");
        highScoreText.text = $"High Score: {highScore} ";
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