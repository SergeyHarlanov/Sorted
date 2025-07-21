using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DraggableObjectPool : MonoPoolableMemoryPool<DraggableObjectSpawnParams, IMemoryPool, DraggableObject> {}

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private List<ShapeData> shapeDatas;
    [SerializeField] private Transform[] startPoints;
    [SerializeField] private Transform[] endPoints;

    private float timer;
    private float currentSpawnInterval;
    private bool isSpawningActive = true;

    private GameManager _gameManager;
    private GameSettings _gameSettings;
    private InputManager _inputManager;
    private DraggableObjectPool _pool; 

    [Inject]
    private void Construct(GameManager gameManager, GameSettings gameSettings, InputManager inputManager, DraggableObjectPool pool)
    {
        _gameManager = gameManager;
        _gameSettings = gameSettings;
        _inputManager = inputManager;
        _pool = pool;

        currentSpawnInterval = _gameSettings.GetRandomFigureSpawnTimeout();
        if (_gameManager != null)
        {
            _gameManager.OnGameOver += OnGameOver;
            _gameManager.OnGameWin += OnGameWin;
        }
    }

    private void SpawnObject()
    {
        if (shapeDatas.Count == 0 || startPoints.Length == 0) return;

        // 1. Prepare the spawn data
        int laneIndex = Random.Range(0, startPoints.Length);
        var spawnParams = new DraggableObjectSpawnParams
        {
            ShapeData = shapeDatas[Random.Range(0, shapeDatas.Count)],
            StartPos = startPoints[laneIndex].position,
            EndPos = endPoints[laneIndex].position,
            MoveSpeed = _gameSettings.GetRandomFigureSpeed(),
            GameManager = _gameManager,
            InputManager = _inputManager
        };

        _pool.Spawn(spawnParams, _pool); 
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

    private void OnDestroy()
    {
        if (_gameManager != null)
        {
            _gameManager.OnGameOver -= OnGameOver;
            _gameManager.OnGameWin -= OnGameWin;
        }
    }
    private void OnGameOver(int finalScore) => isSpawningActive = false;
    private void OnGameWin(int finalScore) => isSpawningActive = false;
}