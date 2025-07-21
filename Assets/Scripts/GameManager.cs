using UnityEngine;
using System;
using Zenject; // Обязательно должен быть!

public class GameManager : MonoBehaviour
{
    private GameSettings _gameSettings; 
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

        InitializeGameData(); 
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
        OnLivesChanged?.Invoke(CurrentLives); 

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
        OnGameOver?.Invoke(CurrentScore); 
    }

    /// <summary>
    /// Обрабатывает состояние "Игрок победил".
    /// </summary>
    private void GameWin()
    {
        Debug.Log("You Win! Final Score: " + CurrentScore);
        OnGameWin?.Invoke(CurrentScore); 
    }
}