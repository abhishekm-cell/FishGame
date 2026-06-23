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
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<OnGameStarted>(OnGameStarted);
        EventBus.Unsubscribe<OnScoreChanged>(OnScoreChanged);
        EventBus.Unsubscribe<OnPlayerAte>(OnPlayerAte);
        EventBus.Unsubscribe<OnGameOver>(OnGameOver);
    }

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void ShowHUD()
    {
        mainMenuPanel.SetActive(false);
        hudPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    private void ShowGameOver(string reason, int score)
    {
        gameOverPanel.SetActive(true);
        hudPanel.SetActive(false);
        gameOverReasonText.text = reason == "hook" ? "Caught by a hook!" : "A bigger fish ate you!";
        finalScoreText.text     = $"Score: {score}";
    }

    private void OnGameStarted(OnGameStarted _)   => ShowHUD();
    private void OnScoreChanged(OnScoreChanged e)  => scoreText.text = $"Score: {e.score}";
    private void OnPlayerAte(OnPlayerAte e)        => sizeText.text  = $"Size: {e.newPlayerSize}";
    private void OnGameOver(OnGameOver e)          => ShowGameOver(e.reason, e.finalScore);

    private void OnPlayPressed()    => _gameManager.StartGame();
    private void OnRestartPressed() => _gameManager.RestartGame();
}
