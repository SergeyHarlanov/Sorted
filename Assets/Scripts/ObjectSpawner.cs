// Файл: ObjectSpawner.cs
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Настройки игры")]
    [Tooltip("Ссылка на ScriptableObject с общими настройками игры")]
    [SerializeField] private GameSettings gameSettings; // <- Добавляем эту ссылку!

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
    private float currentSpawnInterval; // Для хранения рандомного интервала
    private bool isSpawningActive = true; // Флаг для управления спауном, если игра не окончена

    private void Start()
    {
        if (gameSettings == null)
        {
            Debug.LogError("GameSettings не назначен в ObjectSpawner! Пожалуйста, назначьте ScriptableObject GameSettings в инспекторе.");
            enabled = false; // Отключаем скрипт, если нет настроек
            return;
        }

        // Получаем первый случайный интервал спауна из GameSettings
        currentSpawnInterval = gameSettings.GetRandomFigureSpawnTimeout();

        // Подписываемся на события GameManager, чтобы останавливать спаун при конце игры
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += OnGameOver;
            GameManager.Instance.OnGameWin += OnGameWin;
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий, чтобы избежать ошибок при уничтожении объекта
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= OnGameOver;
            GameManager.Instance.OnGameWin -= OnGameWin;
        }
    }

    private void Update()
    {
        if (!isSpawningActive) return; // Не спауним, если флаг выключен

        timer += Time.deltaTime;
        if (timer >= currentSpawnInterval)
        {
            SpawnObject();
            timer = 0;
            // Генерируем новый случайный интервал для следующего спауна
            currentSpawnInterval = gameSettings.GetRandomFigureSpawnTimeout();
        }
    }

    private void SpawnObject()
    {
        if (draggableObjectPrefab == null || shapeDatas.Count == 0 || startPoints.Length == 0)
        {
            Debug.LogError("Не все настройки спаунера заданы!");
            return;
        }

        // 1. Выбираем случайный лайн
        int laneIndex = Random.Range(0, startPoints.Length);

        // 2. Выбираем случайную фигуру
        ShapeData randomShapeData = shapeDatas[Random.Range(0, shapeDatas.Count)];

        // 3. Создаем объект из префаба
        DraggableObject newObject = Instantiate(draggableObjectPrefab, startPoints[laneIndex].position, Quaternion.identity);

        // 4. Инициализируем его данными, используя скорость из GameSettings
        float randomSpeed = gameSettings.GetRandomFigureSpeed(); // <- Берем скорость из GameSettings!
        newObject.Initialize(randomShapeData, startPoints[laneIndex].position, endPoints[laneIndex].position, randomSpeed);
    }

    // Методы для обработки окончания игры и остановки спауна
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