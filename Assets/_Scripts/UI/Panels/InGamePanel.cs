using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGamePanel : MonoBehaviour
{
    // handles In game details like score , pause button etc
    [Header("References")]
    private GameManager gManager;

    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Button")]
    [SerializeField] private Button pauseButton;

    void OnEnable()
    {
        pauseButton.onClick.AddListener(OnPauseClicked);

        Events.UpdateScore += UpdateScore;
    }

    void OnDisable()
    {
        pauseButton.onClick.RemoveListener(OnPauseClicked);

        Events.UpdateScore -= UpdateScore;
    }

    public void SetReferences(GameManager gameManager)
    {
        gManager = gameManager;
        //UpdateScore(0);
        
    }

    void OnPauseClicked()
    {
        Debug.Log("Pause Button Clicked");
        Events.GamePausedInvoke();

    }

    private void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
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