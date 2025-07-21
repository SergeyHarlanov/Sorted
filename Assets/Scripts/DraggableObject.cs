using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class DraggableObject : MonoBehaviour, IPoolable<DraggableObjectSpawnParams, IMemoryPool>
{
    public ShapeData ShapeData => shapeData;
    private ShapeData shapeData;
    private bool isDragging = false;
    private bool isReturning = false;
    private Vector3 targetPosition;
    private Vector3 returnStartPosition;
    private float speed;

    private Rigidbody2D rb;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider2D;
    private Camera mainCamera;
    private GameManager _gameManager;
    private InputManager _inputManager;
    private IMemoryPool _pool; // Ссылка на пул для возврата

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        rb.isKinematic = true;
    }

    /// <summary>
    /// Вызывается, когда объект "просыпается" (берётся из пула).
    /// </summary>
    public void OnSpawned(DraggableObjectSpawnParams p, IMemoryPool pool)
    {
        _pool = pool;
        shapeData = p.ShapeData;
        _spriteRenderer.sprite = shapeData.sprite;
        transform.position = p.StartPos;
        targetPosition = p.EndPos;
        speed = p.MoveSpeed;
        gameObject.name = shapeData.name;
        _gameManager = p.GameManager;
        _inputManager = p.InputManager;

        // Сброс состояния
        isDragging = false;
        isReturning = false;
        gameObject.SetActive(true);
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        // Подписка на события
        if (_inputManager != null)
        {
            _inputManager.OnDragStart += HandleDragStart;
            _inputManager.OnDragEnd += HandleDragEnd;
            _inputManager.OnDragCollectEnd += HandleDragCollectEnd;
        }

        _boxCollider2D = boxCollider;
        boxCollider.enabled = true;
        
        if (spriteRenderer != null && boxCollider != null)
        {
             
            // Устанавливаем размер коллайдера равным размеру спрайта
            boxCollider.size = spriteRenderer.sprite.bounds.size;
            // Сбрасываем смещение, чтобы оно было по центру спрайта
            boxCollider.offset = Vector2.zero; 
        }
    }

    /// <summary>
    /// Вызывается, когда объект "засыпает" (возвращается в пул).
    /// </summary>
    public void OnDespawned()
    {
        // Отписка от событий во избежание утечек
        if (_inputManager != null)
        {
            _inputManager.OnDragStart -= HandleDragStart;
            _inputManager.OnDragEnd -= HandleDragEnd;
            _inputManager.OnDragCollectEnd -= HandleDragCollectEnd;
        }
        _pool = null;
        gameObject.SetActive(false);
    }
    
    // В логике игры заменяем Destroy() на возврат в пул
    private void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            rb.MovePosition(mousePosition);
        }
        else if (isReturning)
        {
            transform.position = Vector3.Lerp(transform.position, returnStartPosition, Time.deltaTime * 8f);
            if (Vector3.Distance(transform.position, returnStartPosition) < 0.1f) isReturning = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                _gameManager.LoseLife();
                _pool.Despawn(this); // Возврат в пул
            }
        }
    }

    private void HandleDragCollectEnd(GameObject droppedObject, Slot slot)
    {
        if (droppedObject != gameObject) return;
        if (slot != null && slot.acceptedShape == this.shapeData.shapeType)
        {
            _gameManager.AddScore();
            _pool.Despawn(this); // Возврат в пул
        }
        else
        {
            if (slot != null) _gameManager.LoseLife();
            HandleDragEnd(droppedObject);
        }
    }

    // Вспомогательные методы остаются без изменений
    private void HandleDragStart(GameObject draggedObject)
    {
        if (draggedObject == gameObject)
        {
            isDragging = true;
            isReturning = false;
            returnStartPosition = transform.position;
            _boxCollider2D.enabled = false;
        }
    }

    private void HandleDragEnd(GameObject droppedObject)
    {
        if (droppedObject == gameObject)
        {
            isDragging = false;
        }
    }
}