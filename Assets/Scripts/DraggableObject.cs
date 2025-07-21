// Файл: DraggableObject.cs

using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class DraggableObject : MonoBehaviour
{
    public ShapeData ShapeData => shapeData;
    // --- Данные и состояние ---
    private ShapeData shapeData;
    private bool isDragging = false;
    private bool isReturning = false;

    // --- Движение ---
    private Vector3 targetPosition;
    private Vector3 returnStartPosition;
    private float speed;

    // --- Компоненты ---
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

     private GameManager _gameManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        // Настраиваем Rigidbody для перетаскивания и движения
        rb.isKinematic = true;
    }

    private void Start()
    {
        // Подписываемся на события InputManager
        InputManager.Instance.OnDragStart += HandleDragStart;
        InputManager.Instance.OnDragEnd += HandleDragEnd;
        InputManager.Instance.OnDragCollectEnd += HandleDragCollectEnd;
       
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        if (spriteRenderer != null && boxCollider != null)
        {
            // Устанавливаем размер коллайдера равным размеру спрайта
            boxCollider.size = spriteRenderer.sprite.bounds.size;
            // Сбрасываем смещение, чтобы оно было по центру спрайта
            boxCollider.offset = Vector2.zero; 
        }
    }

    private void HandleDragCollectEnd(GameObject droppedObject, Slot slot)
    {
        if (droppedObject != gameObject) return;
        Debug.Log("DraggableObject HandleDragCollectEnd1"+(slot.acceptedShape == this.shapeData.shapeType));
        // Столкновение происходит только когда мы отпускаем объект
      //  if (isDragging) return;

        if (slot != null)
        {
            if (slot.acceptedShape == this.shapeData.shapeType)
            {
                // Правильный слот
                _gameManager.AddScore();
                Debug.Log("Правильно! +1 очко.");
                Destroy(gameObject);
            }
            else
            {
                // Неправильный слот
                _gameManager.LoseLife();
                Debug.Log("Неправильно! -1 жизнь.");
                
                // Вызываем событие окончания перетаскивания
                HandleDragEnd(droppedObject);
                // Тут можно добавить эффект взрыва
            
            }
        }
    }
    

    private void OnDestroy()
    {
        // Всегда отписывайтесь от событий, чтобы избежать утечек памяти
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnDragStart -= HandleDragStart;
            InputManager.Instance.OnDragEnd -= HandleDragEnd;
            InputManager.Instance.OnDragCollectEnd -= HandleDragCollectEnd;
        }
    }
    
    // Метод инициализации, вызываемый спаунером
    public void Initialize(ShapeData data, Vector3 startPos, Vector3 endPos, float moveSpeed, GameManager gameManager)
    {
        shapeData = data;
        spriteRenderer.sprite = shapeData.sprite;
        transform.position = startPos;
        targetPosition = endPos;
        speed = moveSpeed;
        gameObject.name = shapeData.name; // Устанавливаем имя для удобства отладки
        _gameManager = gameManager;
    }

    private void Update()
    {
        if (isDragging)
        {
            // Перемещение объекта вслед за курсором
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            rb.MovePosition(mousePosition);
        }
        else if (isReturning)
        {
            // Возвращение на исходную позицию
            transform.position = Vector3.Lerp(transform.position, returnStartPosition, Time.deltaTime * 8f);
            if (Vector3.Distance(transform.position, returnStartPosition) < 0.1f)
            {
                isReturning = false;
            }
        }
        else
        {
            // Обычное движение слева направо
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                _gameManager.LoseLife(); // Фигура дошла до конца
                Destroy(gameObject);
            }
        }
    }

    private void HandleDragStart(GameObject draggedObject)
    {
        if (draggedObject == gameObject)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            isDragging = true;
            isReturning = false;
            returnStartPosition = transform.position; // Запоминаем место, откуда начали тащить
        }
    }

    private void HandleDragEnd(GameObject droppedObject)
    {
        if (droppedObject == gameObject)
        {
            GetComponent<BoxCollider2D>().enabled = true; 
            isDragging = false;
        }
    }
    
    
    // Если после перетаскивания фигура не попала в слот, она должна вернуться на место
    private void OnTriggerExit2D(Collider2D other)
    {
        // Проверяем, что мы отпустили фигуру и она вышла из зоны слота
        if (!isDragging && other.GetComponent<Slot>() != null)
        {
             StartCoroutine(CheckReturnAfterFrame());
        }
    }

    // Небольшая задержка, чтобы убедиться, что мы не вошли в другой слот сразу же
    private System.Collections.IEnumerator CheckReturnAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        // Если мы все еще не перетаскиваемся и не уничтожены - возвращаемся
        if (!isDragging && this != null)
        {
            isReturning = true;
        }
    }
}