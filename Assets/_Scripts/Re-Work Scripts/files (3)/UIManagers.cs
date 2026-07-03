using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManagers : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI sizeText;
 
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
 
    [Header("Game Over refs")]
    [SerializeField] private TextMeshProUGUI gameOverReasonText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
 
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button restartButton;
 
    private GamesManager _gameManager;
 
    void Awake()
    {
        playButton?.onClick.AddListener(OnPlayPressed);
        restartButton?.onClick.AddListener(OnRestartPressed);
        _gameManager = ServiceLocator.Instance.Get<GamesManager>();
    }
 
    void Start()
    {
        ShowMainMenu();
    }
 
    void OnEnable()
    {
        EventBus.Subscribe<OnGameStarted>(OnGameStarted);
        EventBus.Subscribe<OnScoreChanged>(OnScoreChanged);
        EventBus.Subscribe<OnPlayerAte>(OnPlayerAte);
        EventBus.Subscribe<OnGameOver>(OnGameOver);
        EventBus.Subscribe<OnGamePaused>(OnGamePaused);
        EventBus.Subscribe<OnGameResumed>(OnGameResumed);
        EventBus.Subscribe<OnReturnToMainMenu>(OnReturnToMainMenu);
    }
 
    void OnDisable()
    {
        EventBus.Unsubscribe<OnGameStarted>(OnGameStarted);
        EventBus.Unsubscribe<OnScoreChanged>(OnScoreChanged);
        EventBus.Unsubscribe<OnPlayerAte>(OnPlayerAte);
        EventBus.Unsubscribe<OnGameOver>(OnGameOver);
        EventBus.Unsubscribe<OnGamePaused>(OnGamePaused);
        EventBus.Unsubscribe<OnGameResumed>(OnGameResumed);
        EventBus.Unsubscribe<OnReturnToMainMenu>(OnReturnToMainMenu);
    }
 
    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
    }
 
    private void ShowHUD()
    {
        mainMenuPanel.SetActive(false);
        hudPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
    }
 
    private void ShowGameOver(string reason, int score)
    {
        gameOverPanel.SetActive(true);
        hudPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        gameOverReasonText.text = reason == "hook" ? "Caught by a hook!" : "A bigger fish ate you!";
        finalScoreText.text     = $"Score: {score}";
    }
 
    private void OnGameStarted(OnGameStarted _) => ShowHUD();
    private void OnScoreChanged(OnScoreChanged e) => scoreText.text = $"Score: {e.score}";
    private void OnPlayerAte(OnPlayerAte e) => sizeText.text  = $"Size: {e.newPlayerSize}";
    private void OnGameOver(OnGameOver e) => ShowGameOver(e.reason, e.finalScore);
    private void OnGamePaused(OnGamePaused _) => pausePanel?.SetActive(true);
    private void OnGameResumed(OnGameResumed _) => pausePanel?.SetActive(false);
    private void OnReturnToMainMenu(OnReturnToMainMenu _) => ShowMainMenu();
 
    private void OnPlayPressed() => _gameManager.StartGame();
    private void OnRestartPressed() => _gameManager.RestartGame();
}
