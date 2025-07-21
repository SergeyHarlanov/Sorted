// Файл: UIManager.cs
using UnityEngine;
using TMPro; // Для работы с TextMeshPro
using UnityEngine.UI; // Для работы с UI элементами
using UnityEngine.SceneManagement; // Для перезагрузки сцены

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Элементы")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winScoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Изначально скрываем все панели
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
    }

    private void Start()
    {
        // Убеждаемся, что GameManager существует и подписываемся на его события
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScoreDisplay;
            GameManager.Instance.OnLivesChanged += UpdateLivesDisplay;
            GameManager.Instance.OnGameOver += ShowGameOverScreen;
            GameManager.Instance.OnGameWin += ShowWinScreen;

            // Обновляем UI при старте
            UpdateScoreDisplay(GameManager.Instance.score);
            UpdateLivesDisplay(GameManager.Instance.lives);
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий при уничтожении объекта
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
            GameManager.Instance.OnLivesChanged -= UpdateLivesDisplay;
            GameManager.Instance.OnGameOver -= ShowGameOverScreen;
            GameManager.Instance.OnGameWin -= ShowWinScreen;
        }
    }

    public void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + newScore;
        }
    }

    public void UpdateLivesDisplay(int newLives)
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + newLives;
        }
    }

    public void ShowGameOverScreen(int finalScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverScoreText != null)
            {
                gameOverScoreText.text = "Final Score: " + finalScore;
            }
        }
        Time.timeScale = 0f; // Останавливаем игру
    }

    public void ShowWinScreen(int finalScore)
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (winScoreText != null)
            {
                winScoreText.text = "Score: " + finalScore;
            }
        }
        Time.timeScale = 0f; // Останавливаем игру
    }

    // Метод для кнопки "Рестарт"
    public void RestartGame()
    {
        Time.timeScale = 1f; // Возобновляем игру
        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}