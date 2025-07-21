using UnityEngine;
using TMPro; 
using Zenject; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText; 
    [SerializeField] private TextMeshProUGUI _livesText; 

    [SerializeField] private GameObject _resultPanel; 
    [SerializeField] private TextMeshProUGUI _resultText; 
    [SerializeField] private Image _resultPanelBackground; 
    [SerializeField] private Button _restartButton; 

    [SerializeField] private Color _gameOverColor = Color.red; 
    [SerializeField] private Color _winColor = Color.green; 

    private GameManager _gameManager;
    private EventBus _eventBus;

    [Inject]
    public void Construct(GameManager gameManager, EventBus eventBus)
    {
        _gameManager = gameManager;
        _eventBus = eventBus;

        _eventBus.OnScoreChanged += _UpdateScoreUI;
        _eventBus.OnLivesChanged += _UpdateLivesUI;
        _eventBus.OnGameOver += _OnGameOverUI;
        _eventBus.OnGameWin += _OnGameWinUI;

        _restartButton.onClick.AddListener(_OnRestartButtonClicked);

        _HideResultPanel();

        _UpdateScoreUI(_gameManager.CurrentScore);
        _UpdateLivesUI(_gameManager.CurrentLives);
    }

    private void OnDestroy()
    {
        if (_eventBus != null)
        {
            _eventBus.OnScoreChanged -= _UpdateScoreUI;
            _eventBus.OnLivesChanged -= _UpdateLivesUI;
            _eventBus.OnGameOver -= _OnGameOverUI;
            _eventBus.OnGameWin -= _OnGameWinUI;
        }
        if (_restartButton != null)
        {
            _restartButton.onClick.RemoveListener(_OnRestartButtonClicked);
        }
    }

    private void _UpdateScoreUI(int newScore)
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Score: {newScore}/{_gameManager.FiguresRequiredToWin}";
        }
    }

    private void _UpdateLivesUI(int newLives)
    {
        if (_livesText != null)
        {
            _livesText.text = $"Lives: {newLives}";
        }
    }

    private void _OnGameOverUI(int finalScore)
    {
        _UpdateScoreUI(finalScore); 
        _ShowResultPanel("You Lose!", finalScore, _gameOverColor);
    }

    private void _OnGameWinUI(int finalScore)
    {
        _UpdateScoreUI(finalScore);
        _ShowResultPanel("You Win!", finalScore, _winColor);
    }

    private void _ShowResultPanel(string message, int score, Color backgroundColor)
    {
        if (_resultPanel != null)
        {
            _resultPanel.SetActive(true);
            
            if (_resultText != null)
            {
                _resultText.text = $"{message}\nYou score: {score}";
            }

            if (_resultPanelBackground != null)
            {
                _resultPanelBackground.color = backgroundColor;
            }
        }
    }

    private void _HideResultPanel()
    {
        if (_resultPanel != null)
        {
            _resultPanel.SetActive(false);
        }
    }

    public void _OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}