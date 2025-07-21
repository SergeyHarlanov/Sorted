// Файл: GameManager.cs
using UnityEngine;
using System; // Добавлено для Action

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // События для обновления UI
    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;
    public event Action<int> OnGameOver;
    public event Action<int> OnGameWin;

    public int score = 0;
    public int lives = 3;
    public int scoreToWin = 10; // Добавьте это поле, чтобы определить, сколько очков нужно для победы

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore()
    {
        score++;
        Debug.Log("Score: " + score);
        OnScoreChanged?.Invoke(score); // Вызываем событие

        if (score >= scoreToWin)
        {
            GameWin();
        }
    }

    public void LoseLife()
    {
        lives--;
        Debug.Log("Lives left: " + lives);
        OnLivesChanged?.Invoke(lives); // Вызываем событие

        if (lives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over! Final Score: " + score);
        OnGameOver?.Invoke(score); // Вызываем событие
        // Здесь можно добавить логику для паузы игры, сохранения рекордов и т.д.
    }

    private void GameWin()
    {
        Debug.Log("You Win! Final Score: " + score);
        OnGameWin?.Invoke(score); // Вызываем событие
        // Здесь можно добавить логику для паузы игры, сохранения рекордов и т.д.
    }
}