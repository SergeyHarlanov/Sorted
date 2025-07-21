using System;
using UnityEngine;
using Zenject;

public class InputManager : MonoBehaviour
{
    private EventBus _eventBus;
    private GameObject _currentDraggedObject;
    private Camera _mainCamera;
    private bool _isDragging = false;

    [Inject]
    public void Construct(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }

        if (_isDragging && Input.GetMouseButton(0))
        {
            ContinueDrag();
        }

        if (_isDragging && Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    private void StartDrag()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            _mainCamera.ScreenToWorldPoint(Input.mousePosition),
            Vector2.zero);

        if (hit.collider != null)
        {
            DraggableObject draggable = hit.collider.GetComponent<DraggableObject>();
            if (draggable != null)
            {
                _currentDraggedObject = hit.collider.gameObject;
                _isDragging = true;
                _eventBus.PublishDragStart(_currentDraggedObject);
            }
        }
    }

    private void ContinueDrag()
    {
        if (_currentDraggedObject == null) return;

        Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        _currentDraggedObject.transform.position = mousePosition;

        _eventBus.PublishDrag(_currentDraggedObject);
    }

    private void EndDrag()
    {
        if (_currentDraggedObject == null) return;

        RaycastHit2D hit = Physics2D.Raycast(
            _mainCamera.ScreenToWorldPoint(Input.mousePosition),
            Vector2.zero);

        if (hit.collider)
        {
            Slot slot = hit.collider.gameObject.GetComponent<Slot>();
            // Теперь PublishDragCollectEnd будет всегда вызываться с правильными параметрами
            _eventBus.PublishDragCollectEnd(_currentDraggedObject, slot); // Исправлено: передаем slot

            _currentDraggedObject = null;
            _isDragging = false;
        }
        else
        {
            _eventBus.PublishDragEnd(_currentDraggedObject);

            _currentDraggedObject = null;
            _isDragging = false;
        }
    }
}