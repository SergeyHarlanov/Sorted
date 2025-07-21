using UnityEngine;
using System;
using Zenject;

public class GameManager : MonoBehaviour
{
    private GameSettings _gameSettings;
    private EventBus _eventBus;

    public int CurrentLives { get; private set; }
    public int CurrentScore { get; private set; }

    public int FiguresRequiredToWin { get; private set; }

    [Inject]
    public void Construct(GameSettings gameSettings, EventBus eventBus)
    {
        _gameSettings = gameSettings;
        _eventBus = eventBus;
        InitializeGameData();
    }

    private void InitializeGameData()
    {
        if (_gameSettings == null)
        {
            Debug.LogError("GameSettings не инициализирован в GameManager!");
            return;
        }

        CurrentLives = _gameSettings.InitialLives;
        CurrentScore = 0;
        FiguresRequiredToWin = _gameSettings.GetRandomFiguresToWin();

        // Публикуем начальные значения через EventBus для обновления UI
        _eventBus.PublishLivesChanged(CurrentLives);
        _eventBus.PublishScoreChanged(CurrentScore);
    }

    public void AddScore()
    {
        CurrentScore++;
        _eventBus.PublishScoreChanged(CurrentScore);

        if (CurrentScore >= FiguresRequiredToWin)
        {
            GameWin();
        }
    }

    public void LoseLife()
    {
        CurrentLives--;
        _eventBus.PublishLivesChanged(CurrentLives);

        if (CurrentLives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        _eventBus.PublishGameOver(CurrentScore);
    }

    private void GameWin()
    {
        _eventBus.PublishGameWin(CurrentScore);
    }
}