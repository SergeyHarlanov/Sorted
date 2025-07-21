using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DraggableObjectPool : MonoPoolableMemoryPool<DraggableObjectSpawnParams, IMemoryPool, DraggableObject> {}

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private List<ShapeData> _shapeDatas;
    [SerializeField] private Transform[] _startPoints;
    [SerializeField] private Transform[] _endPoints;

    private float _timer;
    private float _currentSpawnInterval;
    private bool _isSpawningActive = true;

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

        _currentSpawnInterval = _gameSettings.GetRandomFigureSpawnTimeout();
        
        _eventBus.OnGameOver += _OnGameOver;
        _eventBus.OnGameWin += _OnGameWin;
    }

    private void _SpawnObject()
    {
        if (_shapeDatas.Count == 0 || _startPoints.Length == 0) return;

        int laneIndex = Random.Range(0, _startPoints.Length);
        var spawnParams = new DraggableObjectSpawnParams
        {
            ShapeData = _shapeDatas[Random.Range(0, _shapeDatas.Count)],
            StartPos = _startPoints[laneIndex].position,
            EndPos = _endPoints[laneIndex].position,
            MoveSpeed = _gameSettings.GetRandomFigureSpeed(),
            GameManager = _gameManager,
            InputManager = _inputManager
        };

        _pool.Spawn(spawnParams, _pool); 
    }
    
    private void Update()
    {
        if (!_isSpawningActive) return;
        _timer += Time.deltaTime;
        if (_timer >= _currentSpawnInterval)
        {
            _SpawnObject();
            _timer = 0;
            _currentSpawnInterval = _gameSettings.GetRandomFigureSpawnTimeout();
        }
    }

    private void _OnGameOver(int finalScore)
    {
        _isSpawningActive = false;
    }

    private void _OnGameWin(int finalScore)
    {
        _isSpawningActive = false;
    }

    private void _OnDestroy()
    {
        if (_eventBus != null)
        {
            _eventBus.OnGameOver -= _OnGameOver;
            _eventBus.OnGameWin -= _OnGameWin;
        }
    }
}