using UnityEngine;
using TMPro; // Для TextMeshPro
using Zenject; // Для Zenject
using UnityEngine.UI; // Для Image и Button
using UnityEngine.SceneManagement; // Для перезагрузки сцены

public class UIManager : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private TextMeshProUGUI scoreText; // Текст для текущих очков
    [SerializeField] private TextMeshProUGUI livesText; // Текст для текущих жизней

    [Header("Панель результатов игры")]
    [Tooltip("Единая панель для отображения результатов победы или поражения.")]
    [SerializeField] private GameObject resultPanel; // Единая панель для результатов
    [Tooltip("Текст для отображения сообщения о результате (победа/поражение) и финального счета.")]
    [SerializeField] private TextMeshProUGUI resultText; // Текстовое поле для результата
    [Tooltip("Изображение фона панели результатов, чтобы менять его цвет.")]
    [SerializeField] private Image resultPanelBackground; // Фон панели для изменения цвета
    [Tooltip("Кнопка для перезапуска игры.")]
    [SerializeField] private Button restartButton; // Кнопка перезапуска

    [Header("Настройки цветов для результата")]
    [Tooltip("Цвет фона панели при поражении.")]
    [SerializeField] private Color gameOverColor = Color.red; // Красный для Game Over
    [Tooltip("Цвет фона панели при победе.")]
    [SerializeField] private Color winColor = Color.green; // Зеленый для победы

    [Header("Настройки цвета текста очков")]
    [Tooltip("Цвет текста очков, когда их недостаточно для победы.")]
    [SerializeField] private Color scoreNotEnoughColor = Color.red; // Красный, когда очков не хватает
    [Tooltip("Цвет текста очков, когда их достаточно для победы.")]
    [SerializeField] private Color scoreEnoughColor = Color.white; // Белый (или другой), когда очков хватает

    private GameManager _gameManager; // Инжектируем GameManager

    /// <summary>
    /// Конструктор, вызываемый Zenject для внедрения зависимостей.
    /// </summary>
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

        _gameManager.OnScoreChanged += UpdateScoreUI;
        _gameManager.OnLivesChanged += UpdateLivesUI;
        _gameManager.OnGameOver += OnGameOverUI; 
        _gameManager.OnGameWin += OnGameWinUI;   
        

        UpdateScoreUI(_gameManager.CurrentScore);
        UpdateLivesUI(_gameManager.CurrentLives);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        HideResultPanel();
    }

    /// <summary>
    /// Вызывается при уничтожении объекта. Отписываемся от событий и слушателей кнопок.
    /// </summary>
    private void OnDestroy()
    {
        if (_gameManager != null)
        {
            _gameManager.OnScoreChanged -= UpdateScoreUI;
            _gameManager.OnLivesChanged -= UpdateLivesUI;
            _gameManager.OnGameOver -= OnGameOverUI;
            _gameManager.OnGameWin -= OnGameWinUI;
        }
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }
    }

    /// <summary>
    /// Обновляет отображение текущего счета, указывает сколько еще требуется для победы 
    /// и меняет цвет текста, если очков недостаточно.
    /// </summary>
    /// <param name="newScore">Новое значение счета.</param>
    private void UpdateScoreUI(int newScore)
    {
        if (scoreText != null && _gameManager != null)
        {
            int requiredToWin = _gameManager.FiguresRequiredToWin;
            
            if (newScore < requiredToWin)
            {
                // Если очков не хватает, используем красный цвет и показываем, сколько осталось
                scoreText.text = $"{newScore} / {requiredToWin - newScore}";
            }
            else
            {
                // Если очков достаточно, используем обычный цвет и показываем только текущий счёт
                scoreText.color = scoreEnoughColor;
              //  scoreText.text = $"Score: {newScore}"; 
            }
        }
    }

    /// <summary>
    /// Обновляет отображение текущего количества жизней.
    /// </summary>
    /// <param name="newLives">Новое количество жизней.</param>
    private void UpdateLivesUI(int newLives)
    {
        if (livesText != null)
        {
            livesText.text = $"{newLives}";
        }
    }

    /// <summary>
    /// Обрабатывает событие "Игра окончена" и отображает панель результатов.
    /// </summary>
    /// <param name="finalScore">Финальный счет игрока.</param>
    private void OnGameOverUI(int finalScore)
    {
        Debug.Log($"UI: Игра окончена с очками: {finalScore}");
        ShowResultPanel("ИГРА ОКОНЧЕНА!", finalScore, gameOverColor);
    }

    /// <summary>
    /// Обрабатывает событие "Игрок победил" и отображает панель результатов.
    /// </summary>
    /// <param name="finalScore">Финальный счет игрока.</param>
    private void OnGameWinUI(int finalScore)
    {
        Debug.Log($"UI: Вы победили с очками: {finalScore}");
        ShowResultPanel("ВЫ ПОБЕДИЛИ!", finalScore, winColor);
    }

    /// <summary>
    /// Отображает панель результатов с заданным сообщением, счетом и цветом фона.
    /// </summary>
    /// <param name="message">Сообщение для отображения.</param>
    /// <param name="score">Финальный счет для отображения.</param>
    /// <param name="backgroundColor">Цвет фона панели.</param>
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

    /// <summary>
    /// Скрывает панель результатов игры.
    /// </summary>
    private void HideResultPanel()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Метод, вызываемый при нажатии кнопки перезапуска.
    /// Перезагружает текущую сцену, фактически перезапуская игру.
    /// </summary>
    public void OnRestartButtonClicked()
    {
        Debug.Log("Кнопка перезапуска нажата. Перезагрузка сцены...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}