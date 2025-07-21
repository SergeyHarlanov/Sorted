// Файл: ObjectSpawner.cs
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Ссылки на объекты")]
    [Tooltip("Префаб объекта, который будет спауниться")]
    [SerializeField] private DraggableObject draggableObjectPrefab;

    [Tooltip("Список всех возможных данных о фигурах")]
    [SerializeField] private List<ShapeData> shapeDatas;

    [Header("Позиции лайнов")]
    [Tooltip("Начальные точки для каждого лайна (слева)")]
    [SerializeField] private Transform[] startPoints;
    
    [Tooltip("Конечные точки для каждого лайна (справа)")]
    [SerializeField] private Transform[] endPoints;

    private float timer;
    private float currentSpawnInterval;
    private bool isSpawningActive = true;

    // --- Injected Dependencies ---
    private GameManager _gameManager;
    private GameSettings _gameSettings;
    private InputManager _inputManager;
    private DraggableObjectFactory _draggableObjectFactory; // Factory is now injected

    [Inject]
    private void Construct(GameManager gameManager, GameSettings gameSettings, InputManager inputManager, DraggableObjectFactory draggableObjectFactory)
    {
        _gameManager = gameManager;
        _gameSettings = gameSettings;
        _inputManager = inputManager;
        _draggableObjectFactory = draggableObjectFactory; // Assign the injected factory

        currentSpawnInterval = _gameSettings.GetRandomFigureSpawnTimeout();

        if (_gameManager != null)
        {
            _gameManager.OnGameOver += OnGameOver;
            _gameManager.OnGameWin += OnGameWin;
        }
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
        {
            _gameManager.OnGameOver -= OnGameOver;
            _gameManager.OnGameWin -= OnGameWin;
        }
    }

    private void Update()
    {
        if (!isSpawningActive) return;

        timer += Time.deltaTime;
        if (timer >= currentSpawnInterval)
        {
            SpawnObject();
            timer = 0;
            currentSpawnInterval = _gameSettings.GetRandomFigureSpawnTimeout();
        }
    }

    /// <summary>
    /// Spawns an object using the DraggableObjectFactory.
    /// </summary>
    private void SpawnObject()
    {
        if (draggableObjectPrefab == null || shapeDatas.Count == 0 || startPoints.Length == 0)
        {
            Debug.LogError("Не все настройки спаунера заданы!");
            return;
        }

        // 1. Select a random lane
        int laneIndex = Random.Range(0, startPoints.Length);

        // 2. Select random shape data
        ShapeData randomShapeData = shapeDatas[Random.Range(0, shapeDatas.Count)];

        // 3. Get a random speed from GameSettings
        float randomSpeed = _gameSettings.GetRandomFigureSpeed();

        // 4. Use the factory to create the object.
        // This single method call replaces the previous Instantiate and Initialize calls.
        _draggableObjectFactory.Create(
            draggableObjectPrefab,
            randomShapeData,
            startPoints[laneIndex].position,
            endPoints[laneIndex].position,
            randomSpeed,
            _gameManager,
            _inputManager
        );
    }

    private void OnGameOver(int finalScore)
    {
        isSpawningActive = false;
        Debug.Log("Спаун остановлен: Game Over");
    }

    private void OnGameWin(int finalScore)
    {
        isSpawningActive = false;
        Debug.Log("Спаун остановлен: Game Win");
    }
}