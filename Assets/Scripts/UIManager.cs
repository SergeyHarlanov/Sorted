// Файл: UIManager.cs
using UnityEngine;
using TMPro; // Для работы с TextMeshPro
using UnityEngine.UI; // Для работы с UI элементами
using UnityEngine.SceneManagement; // Для перезагрузки сцены
using System.Collections; // Для корутин

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Элементы")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private Button _restartButton;
    [Header("Панель результатов")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI combinedResultText; // Единый текстовый элемент для заголовка и счета
    [SerializeField] private Image resultPanelBackground; // Фон панели для изменения цвета
    [SerializeField] private Color winColor = Color.green; // Цвет для победы
    [SerializeField] private Color loseColor = Color.red; // Цвет для поражения

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

        // Изначально скрываем панель результатов
        resultPanel.SetActive(false);
    }

    private void Start()
    {
        // Убеждаемся, что GameManager существует и подписываемся на его события
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScoreDisplay;
            GameManager.Instance.OnLivesChanged += UpdateLivesDisplay;
            GameManager.Instance.OnGameOver += ShowResultScreen;
            GameManager.Instance.OnGameWin += ShowResultScreen;
            _restartButton.onClick.AddListener(RestartGame);

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
            GameManager.Instance.OnGameOver -= ShowResultScreen;
            GameManager.Instance.OnGameWin -= ShowResultScreen;
            
            _restartButton.onClick.RemoveListener(RestartGame);
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

    // Универсальный метод для показа экрана результатов
    public void ShowResultScreen(int finalScore)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);

            // Определяем, это победа или поражение
            // Используем условие, что победа достигается, если finalScore >= scoreToWin
            bool isWin = (finalScore >= GameManager.Instance.scoreToWin);

            string title;
            Color panelColor;

            if (isWin)
            {
                title = "Победа!";
                panelColor = winColor;
            }
            else
            {
                title = "Поражение";
                panelColor = loseColor;
            }

            // Объединяем заголовок и счет в одну строку
            if (combinedResultText != null)
            {
                combinedResultText.text = $"{title}\nОчки: {finalScore}";
            }
            
            // Устанавливаем цвет фона панели
            if (resultPanelBackground != null)
            {
                resultPanelBackground.color = panelColor;
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