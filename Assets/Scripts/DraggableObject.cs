// Файл: DraggableObject.cs

using System;
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
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider2D;
    
    private Camera mainCamera;

     private GameManager _gameManager;
     private InputManager _inputManager;


     private void Awake()
     {
         rb = GetComponent<Rigidbody2D>();
         mainCamera = Camera.main;
         
         rb.isKinematic = true;
         
         _spriteRenderer = GetComponent<SpriteRenderer>();
         _boxCollider2D = GetComponent<BoxCollider2D>();
   
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
        if (_inputManager != null)
        {
            _inputManager.OnDragStart -= HandleDragStart;
            _inputManager.OnDragEnd -= HandleDragEnd;
            _inputManager.OnDragCollectEnd -= HandleDragCollectEnd;
        }
    }
    
    // Метод инициализации, вызываемый спаунером
    public void Initialize(ShapeData data, Vector3 startPos, Vector3 endPos, float moveSpeed, GameManager gameManager, InputManager inputManager)
    {
        _inputManager = inputManager;

        // Подписываемся на события InputManager
        _inputManager.OnDragStart += HandleDragStart;
        _inputManager.OnDragEnd += HandleDragEnd;
        _inputManager.OnDragCollectEnd += HandleDragCollectEnd;
        
        shapeData = data;
        _spriteRenderer.sprite = shapeData.sprite;
        transform.position = startPos;
        targetPosition = endPos;
        speed = moveSpeed;
        gameObject.name = shapeData.name; // Устанавливаем имя для удобства отладки
        _gameManager = gameManager;
        
        if (_spriteRenderer != null && _boxCollider2D != null)
        {
             
            // Устанавливаем размер коллайдера равным размеру спрайта
            _boxCollider2D.size = _spriteRenderer.sprite.bounds.size;
            // Сбрасываем смещение, чтобы оно было по центру спрайта
            _boxCollider2D.offset = Vector2.zero; 
        }
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