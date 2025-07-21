// Файл: ObjectSpawner.cs
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Настройки спауна")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 4f;

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

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnObject();
            timer = 0;
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

        // 4. Инициализируем его данными
        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        newObject.Initialize(randomShapeData, startPoints[laneIndex].position, endPoints[laneIndex].position, randomSpeed);
    }
}