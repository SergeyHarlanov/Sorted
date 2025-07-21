using UnityEngine;
using System;
using Zenject; // Обязательно должен быть!

public class GameManager : MonoBehaviour
{
    // Статический Instance (синглтон) для удобства доступа.
    // Zenject-инъекции предпочтительнее для новых зависимостей.
    public static GameManager Instance { get; private set; } 

    // Приватное поле для инжектированных настроек игры.
    private GameSettings _gameSettings; 

    // Публичные события для оповещения UI и других систем об изменениях
    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;
    public event Action<int> OnGameOver;
    public event Action<int> OnGameWin;

    // Публичные свойства для текущего счета, жизней и закешированной цели победы
    public int CurrentLives { get; private set; }
    public int CurrentScore { get; private set; }
    
    // Закешированное количество фигур, которое нужно отсортировать для победы. 
    public int FiguresRequiredToWin { get; private set; } 

    /// <summary>
    /// Основной метод для внедрения зависимостей Zenject.
    /// Zenject вызывает его, когда все зависимости готовы.
    /// </summary>
    [Inject]
    public void Construct(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
        Debug.Log("GameSettings успешно инжектированы в GameManager.");

        // 🔥 Инициализация игровых переменных после инъекций 🔥
        // Это место идеально подходит для инициализации, которая требует _gameSettings
        InitializeGameData(); 
    }

    /// <summary>
    /// Метод Awake вызывается при загрузке скрипта.
    /// Используется для инициализации синглтона.
    /// </summary>
    private void Awake()
    {
        // Проверяем, существует ли уже экземпляр GameManager
        if (Instance == null)
        {
            Instance = this; // Если нет, устанавливаем этот экземпляр как Instance
            // DontDestroyOnLoad(gameObject); // Раскомментируйте, если GameManager должен сохраняться между сценами
        }
        else
        {
            // Если экземпляр уже существует, уничтожаем текущий, чтобы избежать дублирования
            Destroy(gameObject);
        }
        
        // ВАЖНО: На этапе Awake() Zenject еще НЕ гарантирует, что все зависимости (например, _gameSettings) инжектированы.
        // Поэтому основная игровая инициализация происходит в Construct() или методе, вызываемом из него.
    }

    /// <summary>
    /// Отдельный метод для инициализации игровых данных, вызываемый после внедрения зависимостей.
    /// Это делает код чище и разделяет инъекцию от логики инициализации.
    /// </summary>
    private void InitializeGameData()
    {
        if (_gameSettings == null)
        {
            Debug.LogError("Ошибка: GameSettings не был инжектирован в GameManager. Проверьте ваш GameInstaller!");
            enabled = false; 
            return;
        }

        // Инициализируем фиксированную цель для победы ОДИН РАЗ при старте игры
        FiguresRequiredToWin = _gameSettings.GetRandomFiguresToWin();
        
        // Инициализируем текущее здоровье игрока из настроек
        CurrentLives = _gameSettings.PlayerStartingHealth;
        // Сбрасываем текущий счет
        CurrentScore = 0;

        Debug.Log($"Начальное здоровье игрока: {CurrentLives}");
        Debug.Log($"Для победы нужно отсортировать фигур: {FiguresRequiredToWin}"); 

        // Оповещаем UI и другие подписанные системы об начальных значениях
        OnLivesChanged?.Invoke(CurrentLives); 
        OnScoreChanged?.Invoke(CurrentScore); 
    }

    /// <summary>
    /// Увеличивает счет игрока и проверяет условие победы.
    /// </summary>
    public void AddScore()
    {
        CurrentScore++;
        Debug.Log("Score: " + CurrentScore);
        OnScoreChanged?.Invoke(CurrentScore); // Вызываем событие для обновления UI

        // Проверяем, достигнута ли цель победы
        if (CurrentScore >= FiguresRequiredToWin) 
        {
            GameWin();
        }
    }

    /// <summary>
    /// Уменьшает количество жизней игрока и проверяет условие поражения.
    /// </summary>
    public void LoseLife()
    {
        CurrentLives--;
        Debug.Log("Lives left: " + CurrentLives);
        OnLivesChanged?.Invoke(CurrentLives); // Вызываем событие для обновления UI

        // Проверяем, закончились ли жизни
        if (CurrentLives <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Обрабатывает состояние "Игра окончена".
    /// </summary>
    private void GameOver()
    {
        Debug.Log("Game Over! Final Score: " + CurrentScore);
        OnGameOver?.Invoke(CurrentScore); // Вызываем событие
        // Здесь можно добавить логику для:
        // - Паузы игры
        // - Отображения экрана "Game Over"
        // - Сохранения рекордов
        // - Перезапуска сцены
    }

    /// <summary>
    /// Обрабатывает состояние "Игрок победил".
    /// </summary>
    private void GameWin()
    {
        Debug.Log("You Win! Final Score: " + CurrentScore);
        OnGameWin?.Invoke(CurrentScore); // Вызываем событие
        // Здесь можно добавить логику для:
        // - Паузы игры
        // - Отображения экрана "Победа"
        // - Перехода на следующий уровень
    }
}