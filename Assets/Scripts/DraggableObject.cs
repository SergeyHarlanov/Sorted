using UnityEngine;
using Zenject;
using DG.Tweening;

public class DraggableObject : MonoBehaviour, IPoolable<DraggableObjectSpawnParams, IMemoryPool>
{
    private ShapeData shapeData;
    private bool isDragging = false;
    private bool isReturning = false;
    private Vector3 targetPosition;
    private Vector3 returnStartPosition;
    private float speed;

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
        shapeData = p.ShapeData;
        _spriteRenderer.sprite = shapeData.sprite;
        transform.position = p.StartPos;
        targetPosition = p.EndPos;
        speed = p.MoveSpeed;
        _gameManager = p.GameManager;
        _inputManager = p.InputManager;

        _eventBus.OnDragStart += HandleDragStart;
        _eventBus.OnDragEnd += HandleDragEnd;
        _eventBus.OnDragCollectEnd += HandleDragCollectEnd; // Исправлено: прямая подписка
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _boxCollider2D.enabled = true; // Убедиться, что коллайдер включен при спавне

        _boxCollider2D.size = _spriteRenderer.sprite.bounds.size;
        _boxCollider2D.offset = Vector2.zero;

        isDragging = false;
        isReturning = false;
        transform.localScale = _startSize;
    }

    public void OnDespawned()
    {
        if (_eventBus != null)
        {
            _eventBus.OnDragStart -= HandleDragStart;
            _eventBus.OnDragEnd -= HandleDragEnd;
            _eventBus.OnDragCollectEnd -= HandleDragCollectEnd; // Исправлено
        }
    }

    private void Update()
    {
        if (!isDragging && !isReturning)
        {
            MoveTowardsTarget();
        }

        if (isReturning)
        {
            ReturnToStartPosition();
        }
    }

    private void SetRandomRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            _gameManager.LoseLife();
            _pool.Despawn(this);
        }
    }


    private void HandleDragCollectEnd(GameObject droppedObject, Slot slot) // Теперь параметры приходят напрямую
    {
        if (droppedObject != gameObject) return;

        if (slot != null && slot.acceptedShape == this.shapeData.shapeType)
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
            HandleDragEnd(droppedObject); // Объект возвращается на место или удаляется
        }
    }

    private void HandleDragStart(GameObject draggedObject)
    {
        if (draggedObject == gameObject)
        {
            isDragging = true;
            isReturning = false;
            returnStartPosition = transform.position;
            _boxCollider2D.enabled = false; // Отключаем коллайдер при начале перетаскивания

            transform.DOPunchScale(new Vector3(-0.2f, -0.2f, 0), 0.25f, 1, 0.5f)
                .SetEase(Ease.OutBounce);
        }
    }

    private void HandleDragEnd(GameObject droppedObject)
    {
        if (droppedObject == gameObject)
        {
            isDragging = false;
            isReturning = true;
            _boxCollider2D.enabled = true; // Включаем коллайдер после окончания перетаскивания
        }
    }

    private void ReturnToStartPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, returnStartPosition, speed * Time.deltaTime * 2);

        if (Vector3.Distance(transform.position, returnStartPosition) < 0.01f)
        {
            isReturning = false;
        }
    }

    public class Factory : PlaceholderFactory<DraggableObjectSpawnParams, DraggableObject> { }
}