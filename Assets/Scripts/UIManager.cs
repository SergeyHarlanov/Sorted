// Файл: UIManager.cs
using UnityEngine;
using TMPro; // Если используете TextMeshPro для UI
using Zenject; // Добавляем using Zenject

public class UIManager : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI winGoalText; // Новый TextMeshPro для отображения цели победы

    private GameManager _gameManager; // Инжектируем GameManager

    // Zenject инжектирует GameManager
    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
        Debug.Log("GameManager успешно инжектирован в UIManager.");
        
        if (_gameManager == null)
        {
            Debug.LogError("GameManager не был инжектирован в UIManager. Проверьте GameInstaller.");
            enabled = false;
            return;
        }

        // Подписываемся на события GameManager для обновления UI
        _gameManager.OnScoreChanged += UpdateScoreUI;
        _gameManager.OnLivesChanged += UpdateLivesUI;
        _gameManager.OnGameOver += OnGameOverUI; // Опционально: обработка Game Over в UI
        _gameManager.OnGameWin += OnGameWinUI;   // Опционально: обработка Win в UI

        // 🔥 Получаем закешированное количество фигур для победы из GameManager 🔥
        if (winGoalText != null)
        {
            winGoalText.text = $"Цель: {_gameManager.FiguresRequiredToWin}";
        }

        // Инициализируем UI текущими значениями (они уже установлены в GameManager.PostConstruct)
        UpdateScoreUI(_gameManager.CurrentScore);
        UpdateLivesUI(_gameManager.CurrentLives);
    }

    private void OnDestroy()
    {
        // Отписываемся от событий, чтобы избежать утечек памяти и ошибок
        if (_gameManager != null)
        {
            _gameManager.OnScoreChanged -= UpdateScoreUI;
            _gameManager.OnLivesChanged -= UpdateLivesUI;
            _gameManager.OnGameOver -= OnGameOverUI;
            _gameManager.OnGameWin -= OnGameWinUI;
        }
    }

    private void UpdateScoreUI(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Очки: {newScore}";
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
        // Например, показать панель "Game Over"
        Debug.Log($"UI: Игра окончена с очками: {finalScore}");
    }

    private void OnGameWinUI(int finalScore)
    {
        // Например, показать панель "Вы победили!"
        Debug.Log($"UI: Вы победили с очками: {finalScore}");
    }
}