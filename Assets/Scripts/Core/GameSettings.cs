using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Game Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("Количество фигур для победы")]
    [Tooltip("Минимальное количество фигур, которое нужно отсортировать для победы.")]
    [SerializeField] private int _minFiguresToWin = 10;
    [Tooltip("Максимальное количество фигур, которое нужно отсортировать для победы.")]
    [SerializeField] private int _maxFiguresToWin = 20;

    public int GetRandomFiguresToWin() => Random.Range(_minFiguresToWin, _maxFiguresToWin + 1);

    [Header("Таймаут появления фигур")]
    [Tooltip("Минимальное время в секундах до появления следующей фигуры.")]
    [SerializeField] private float _minFigureSpawnTimeout = 0.3f;
    [Tooltip("Максимальное время в секундах до появления следующей фигуры.")]
    [SerializeField] private float _maxFigureSpawnTimeout = 1f;

    public float GetRandomFigureSpawnTimeout() => Random.Range(_minFigureSpawnTimeout, _maxFigureSpawnTimeout);

    [Header("Скорость движения фигур")]
    [Tooltip("Минимальная скорость движения фигуры.")]
    [SerializeField] private float _minFigureSpeed = 2.0f;
    [Tooltip("Максимальная скорость движения фигуры.")]
    [SerializeField] private float _maxFigureSpeed = 5.0f;

    public float GetRandomFigureSpeed() => Random.Range(_minFigureSpeed, _maxFigureSpeed);

    [Header("Здоровье игрока")]
    [Tooltip("Начальное количество здоровья игрока.")]
    [SerializeField] private int _initialLives = 50;

    public int InitialLives => _initialLives;
}