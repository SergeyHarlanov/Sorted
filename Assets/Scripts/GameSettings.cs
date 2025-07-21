using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Game Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("Количество фигур для победы")]
    [Tooltip("Минимальное количество фигур, которое нужно отсортировать для победы.")]
    [SerializeField] private int minFiguresToWin = 10;
    [Tooltip("Максимальное количество фигур, которое нужно отсортировать для победы.")]
    [SerializeField] private int maxFiguresToWin = 20;

    public int GetRandomFiguresToWin() => Random.Range(minFiguresToWin, maxFiguresToWin + 1);

    [Header("Таймаут появления фигур")]
    [Tooltip("Минимальное время в секундах до появления следующей фигуры.")]
    [SerializeField] private float minFigureSpawnTimeout = 1.0f;
    [Tooltip("Максимальное время в секундах до появления следующей фигуры.")]
    [SerializeField] private float maxFigureSpawnTimeout = 3.0f;

    public float GetRandomFigureSpawnTimeout() => Random.Range(minFigureSpawnTimeout, maxFigureSpawnTimeout);

    [Header("Скорость движения фигур")]
    [Tooltip("Минимальная скорость движения фигуры.")]
    [SerializeField] private float minFigureSpeed = 2.0f;
    [Tooltip("Максимальная скорость движения фигуры.")]
    [SerializeField] private float maxFigureSpeed = 5.0f;

    public float GetRandomFigureSpeed() => Random.Range(minFigureSpeed, maxFigureSpeed);

    [Header("Здоровье игрока")]
    [Tooltip("Начальное количество здоровья игрока.")]
    [SerializeField] private int initialLives = 3;

    public int InitialLives => initialLives;
}