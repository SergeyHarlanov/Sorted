using UnityEngine;
using Zenject;
using DG.Tweening;

public class DraggableObject : MonoBehaviour, IPoolable<DraggableObjectSpawnParams, IMemoryPool>
{
    private ShapeData _shapeData;
    private bool _isDragging = false;
    private bool _isReturning = false;
    private Vector3 _targetPosition;
    private Vector3 _returnStartPosition;
    private float _speed;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider2D;
    private GameManager _gameManager;
    private InputManager _inputManager;
    private EventBus _eventBus;
    private IMemoryPool _pool;

    private Vector3 _startSize;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb.isKinematic = true;
        _startSize = transform.localScale;
    }

    [Inject]
    public void Construct(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void OnSpawned(DraggableObjectSpawnParams p, IMemoryPool pool)
    {
        _pool = pool;
        _shapeData = p.ShapeData;
        _spriteRenderer.sprite = _shapeData.sprite;
        transform.position = p.StartPos;
        _targetPosition = p.EndPos;
        _speed = p.MoveSpeed;
        _gameManager = p.GameManager;
        _inputManager = p.InputManager;

        _eventBus.OnDragStart += _HandleDragStart;
        _eventBus.OnDragEnd += _HandleDragEnd;
        _eventBus.OnDragCollectEnd += _HandleDragCollectEnd;
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _boxCollider2D.enabled = true;

        _boxCollider2D.size = _spriteRenderer.sprite.bounds.size;
        _boxCollider2D.offset = Vector2.zero;

        _isDragging = false;
        _isReturning = false;
        transform.localScale = _startSize;
    }

    public void OnDespawned()
    {
        if (_eventBus != null)
        {
            _eventBus.OnDragStart -= _HandleDragStart;
            _eventBus.OnDragEnd -= _HandleDragEnd;
            _eventBus.OnDragCollectEnd -= _HandleDragCollectEnd;
        }
    }

    private void Update()
    {
        if (!_isDragging && !_isReturning)
        {
            _MoveTowardsTarget();
        }

        if (_isReturning)
        {
            _ReturnToStartPosition();
        }
    }

    private void _MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
        {
            _gameManager.LoseLife();
            _pool.Despawn(this);
        }
    }

    private void _HandleDragCollectEnd(GameObject droppedObject, Slot slot)
    {
        if (droppedObject != gameObject) return;

        if (slot != null && slot.acceptedShape == this._shapeData.shapeType)
        {
            transform.DOScale(Vector3.zero, 0.2f)
                .OnComplete(() =>
                {
                    _gameManager.AddScore();
                    _pool.Despawn(this);
                });
        }
        else
        {
            if (slot != null) _gameManager.LoseLife();
            _HandleDragEnd(droppedObject);
        }
    }

    private void _HandleDragStart(GameObject draggedObject)
    {
        if (draggedObject == gameObject)
        {
            _isDragging = true;
            _isReturning = false;
            _returnStartPosition = transform.position;
            _boxCollider2D.enabled = false;

            transform.DOPunchScale(new Vector3(-0.1f, -0.1f, 0), 0.25f, 1, 0.1f)
                .SetEase(Ease.OutBounce);
        }
    }

    private void _HandleDragEnd(GameObject droppedObject)
    {
        if (droppedObject == gameObject)
        {
            _isDragging = false;
            _isReturning = true;
            _boxCollider2D.enabled = true;
        }
    }

    private void _ReturnToStartPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, _returnStartPosition, _speed * Time.deltaTime * 2);

        if (Vector3.Distance(transform.position, _returnStartPosition) < 0.01f)
        {
            _isReturning = false;
        }
    }

    public class Factory : PlaceholderFactory<DraggableObjectSpawnParams, DraggableObject> { }
}