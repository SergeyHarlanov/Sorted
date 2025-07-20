using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public int score = 0;
    public int lives = 3;
    
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
    }
    
    public void LoseLife()
    {
        lives--;
        Debug.Log("Lives left: " + lives);
        
        if (lives <= 0)
        {
            GameOver();
        }
    }
    
    private void GameOver()
    {
        Debug.Log("Game Over! Final Score: " + score);
        // Здесь можно добавить перезагрузку сцены или показ меню
    }
}