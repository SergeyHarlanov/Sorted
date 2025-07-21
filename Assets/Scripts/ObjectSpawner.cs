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

    private EventBus _eventBus;
    private GameManager _gameManager;
    private GameSettings _gameSettings;
    private InputManager _inputManager;
    private DraggableObjectPool _pool; 

    [Inject]
    private void Construct(GameManager gameManager, GameSettings gameSettings, InputManager inputManager, DraggableObjectPool pool, EventBus eventBus)
    {
        _gameManager = gameManager;
        _gameSettings = gameSettings;
        _inputManager = inputManager;
        _pool = pool;
        _eventBus = eventBus;

        currentSpawnInterval = _gameSettings.GetRandomFigureSpawnTimeout();
        
        _eventBus.OnGameOver += OnGameOver;
        _eventBus.OnGameWin += OnGameWin;
    }

    private void SpawnObject()
    {
        if (shapeDatas.Count == 0 || startPoints.Length == 0) return;

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

    private void OnGameOver(int finalScore)
    {
        isSpawningActive = false;
    }

    private void OnGameWin(int finalScore)
    {
        isSpawningActive = false;
    }

    private void OnDestroy()
    {
        if (_eventBus != null)
        {
            _eventBus.OnGameOver -= OnGameOver;
            _eventBus.OnGameWin -= OnGameWin;
        }
    }
}