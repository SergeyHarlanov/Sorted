using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    // Делегаты для событий ввода
    public delegate void DragEvent(GameObject draggedObject);
    public delegate void DropEvent(GameObject droppedObject);
    
    // События
    public event DragEvent OnDragStart;
    public event DragEvent OnDragEnd;
    public event DragEvent OnDrag;
    
    private GameObject currentDraggedObject;
    private Camera mainCamera;
    private bool isDragging = false;

    private void Awake()
    {
        // Реализация Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Обработка начала перетаскивания
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }
        
        // Обработка процесса перетаскивания
        if (isDragging && Input.GetMouseButton(0))
        {
            ContinueDrag();
        }
        
        // Обработка окончания перетаскивания
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    private void StartDrag()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            mainCamera.ScreenToWorldPoint(Input.mousePosition), 
            Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.GetComponent<DraggableObject>() != null)
        {
            currentDraggedObject = hit.collider.gameObject;
            isDragging = true;
            
            // Вызываем событие начала перетаскивания
            OnDragStart?.Invoke(currentDraggedObject);
        }
    }

    private void ContinueDrag()
    {
        if (currentDraggedObject == null) return;
        
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        currentDraggedObject.transform.position = mousePosition;
        
        // Вызываем событие продолжения перетаскивания
        OnDrag?.Invoke(currentDraggedObject);
    }

    private void EndDrag()
    {
        if (currentDraggedObject == null) return;
        
        // Вызываем событие окончания перетаскивания
        OnDragEnd?.Invoke(currentDraggedObject);
        
        currentDraggedObject = null;
        isDragging = false;
    }

    // Метод для проверки, происходит ли сейчас перетаскивание
    public bool IsDragging()
    {
        return isDragging;
    }

    // Метод для получения текущего перетаскиваемого объекта
    public GameObject GetCurrentDraggedObject()
    {
        return currentDraggedObject;
    }
}