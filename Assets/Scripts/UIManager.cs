using UnityEngine;
using TMPro; 
using Zenject; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText; 
    [SerializeField] private TextMeshProUGUI livesText; 

    [SerializeField] private GameObject resultPanel; 
    [SerializeField] private TextMeshProUGUI resultText; 
    [SerializeField] private Image resultPanelBackground; 
    [SerializeField] private Button restartButton; 

    [SerializeField] private Color gameOverColor = Color.red; 
    [SerializeField] private Color winColor = Color.green; 

    private GameManager _gameManager;
    private EventBus _eventBus;

    [Inject]
    public void Construct(GameManager gameManager, EventBus eventBus)
    {
        _gameManager = gameManager;
        _eventBus = eventBus;

        _eventBus.OnScoreChanged += UpdateScoreUI;
        _eventBus.OnLivesChanged += UpdateLivesUI;
        _eventBus.OnGameOver += OnGameOverUI;
        _eventBus.OnGameWin += OnGameWinUI;

        restartButton.onClick.AddListener(OnRestartButtonClicked);

        HideResultPanel();

        //при старте обновляем инормацию 
        UpdateScoreUI(_gameManager.CurrentScore);
        UpdateLivesUI(_gameManager.CurrentLives);
    }

    private void OnDestroy()
    {
        if (_eventBus != null)
        {
            _eventBus.OnScoreChanged -= UpdateScoreUI;
            _eventBus.OnLivesChanged -= UpdateLivesUI;
            _eventBus.OnGameOver -= OnGameOverUI;
            _eventBus.OnGameWin -= OnGameWinUI;
        }
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }
    }

    private void UpdateScoreUI(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Счет: {newScore}/{_gameManager.FiguresRequiredToWin}";
        }
    }

    private void UpdateLivesUI(int newLives)
    {
        if (livesText != null)
        {
            livesText.text = $"Жизни: {newLives}";
        }
    }

    private void OnGameOverUI(int finalScore)
    {
        UpdateScoreUI(finalScore); 
        ShowResultPanel("ИГРА ОКОНЧЕНА!", finalScore, gameOverColor);
    }

    private void OnGameWinUI(int finalScore)
    {
        UpdateScoreUI(finalScore);
        ShowResultPanel("ВЫ ПОБЕДИЛИ!", finalScore, winColor);
    }

    private void ShowResultPanel(string message, int score, Color backgroundColor)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
            
            if (resultText != null)
            {
                resultText.text = $"{message}\nВаш счет: {score}";
            }

            if (resultPanelBackground != null)
            {
                resultPanelBackground.color = backgroundColor;
            }
        }
    }

    private void HideResultPanel()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}