using System;
using UnityEngine;
using Zenject;

public class InputManager : MonoBehaviour
{
    private EventBus _eventBus;
    private GameObject currentDraggedObject;
    private Camera mainCamera;
    private bool isDragging = false;

    [Inject]
    public void Construct(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    private void Start()
    {
        mainCamera = Camera.main;
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

        if (isDragging && Input.GetMouseButton(0))
        {
            ContinueDrag();
        }

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

        if (hit.collider != null)
        {
            DraggableObject draggable = hit.collider.GetComponent<DraggableObject>();
            if (draggable != null)
            {
                currentDraggedObject = hit.collider.gameObject;
                isDragging = true;
                _eventBus.PublishDragStart(currentDraggedObject);
            }
        }
    }

    private void ContinueDrag()
    {
        if (currentDraggedObject == null) return;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        currentDraggedObject.transform.position = mousePosition;

        _eventBus.PublishDrag(currentDraggedObject);
    }

    private void EndDrag()
    {
        if (currentDraggedObject == null) return;

        RaycastHit2D hit = Physics2D.Raycast(
            mainCamera.ScreenToWorldPoint(Input.mousePosition),
            Vector2.zero);

        if (hit.collider)
        {
            Slot slot = hit.collider.gameObject.GetComponent<Slot>();
            // Теперь PublishDragCollectEnd будет всегда вызываться с правильными параметрами
            _eventBus.PublishDragCollectEnd(currentDraggedObject, slot); // Исправлено: передаем slot

            currentDraggedObject = null;
            isDragging = false;
        }
        else
        {
            _eventBus.PublishDragEnd(currentDraggedObject);

            currentDraggedObject = null;
            isDragging = false;
        }
    }
}