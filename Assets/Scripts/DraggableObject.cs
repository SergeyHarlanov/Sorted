using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isDragging = false;
    private float speed;
    private bool isReturning = false;
    private bool isCorrectSlot = false;
    
    public float minSpeed = 1f;
    public float maxSpeed = 5f;

    private Vector3 _currentTargetPosition;
    
    // Ссылка на камеру для преобразования координат
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;
        speed = Random.Range(minSpeed, maxSpeed);
        
        // Подписываемся на события InputManager
        InputManager.Instance.OnDragStart += HandleDragStart;
        InputManager.Instance.OnDragEnd += HandleDragEnd;
        InputManager.Instance.OnDragCollectEnd += HandleDragCollectEnd;
    }

    private void HandleDragCollectEnd(GameObject draggedobject)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Хорошая практика - отписываться от событий при уничтожении объекта
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnDragStart -= HandleDragStart;
            InputManager.Instance.OnDragEnd -= HandleDragEnd;
            InputManager.Instance.OnDragCollectEnd -= HandleDragCollectEnd;
        }
    }
    
    private void Update()
    {
        if (isDragging)
        {
            // Если объект перетаскивается, он следует за курсором мыши
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            transform.position = mousePosition;
        }
        else if (!isReturning)
        {
            // Логика автоматического движения слева направо
            Vector3 direction = (_currentTargetPosition - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
            
            float distance = Vector3.Distance(transform.position, _currentTargetPosition);
            
            // Проверка достижения правого края (Death Zone)
            if (distance < 0.1f)
            {
                GameManager.Instance.LoseLife();
                Destroy(gameObject);
            }
        }
    }
    
    private System.Collections.IEnumerator ReturnToOriginalPosition()
    {
        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * 5f);
            yield return null;
        }
        
        transform.position = originalPosition;
        isReturning = false;
        // После возврата объект должен снова начать двигаться
        isDragging = false; 
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Эта логика теперь должна работать правильно
        if (!isDragging) return; // Срабатывать только во время перетаскивания

        Debug.Log("OnTriggerEnter2D с " + other.name);
        if (other.CompareTag("CorrectSlot"))
        {
            isCorrectSlot = true;
            GameManager.Instance.AddScore();
            Destroy(gameObject);
        }
        else if (other.CompareTag("WrongSlot"))
        {
            GameManager.Instance.LoseLife();
            Destroy(gameObject);
            // Здесь можно добавить эффект взрыва
        }
    }
    
    private void HandleDragStart(GameObject draggedObject)
    {
        if (draggedObject == gameObject)
        {
            Debug.Log(gameObject.name + " был взят.");
            isDragging = true;
            isReturning = false;
            // Сохраняем позицию, с которой начали тащить
            originalPosition = transform.position;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void HandleDragEnd(GameObject droppedObject)
    {
        if (droppedObject == gameObject)
        {
            Debug.Log(gameObject.name + " был отпущен.");
            isDragging = false;
            
            if (!isCorrectSlot)
            {
                isReturning = true;
                StartCoroutine(ReturnToOriginalPosition());
            }
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    public void SetData(Vector3 currentTargetPosition)
    {
        _currentTargetPosition = currentTargetPosition;
    }
}