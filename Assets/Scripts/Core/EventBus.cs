using System;
using UnityEngine;
using Zenject;

public class EventBus
{
    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;
    public event Action<int> OnGameOver;
    public event Action<int> OnGameWin;

    public event Action<GameObject> OnDragStart;
    public event Action<GameObject> OnDragEnd;
    public event Action<GameObject, Slot> OnDragCollectEnd; // Исправлено: теперь передает GameObject и Slot
    public event Action<GameObject> OnDrag;

    public void PublishScoreChanged(int newScore)
    {
        OnScoreChanged?.Invoke(newScore);
    }

    public void PublishLivesChanged(int newLives)
    {
        OnLivesChanged?.Invoke(newLives);
    }

    public void PublishGameOver(int finalScore)
    {
        OnGameOver?.Invoke(finalScore);
    }

    public void PublishGameWin(int finalScore)
    {
        OnGameWin?.Invoke(finalScore);
    }

    public void PublishDragStart(GameObject draggedObject)
    {
        OnDragStart?.Invoke(draggedObject);
    }

    public void PublishDragEnd(GameObject droppedObject)
    {
        OnDragEnd?.Invoke(droppedObject);
    }

    public void PublishDragCollectEnd(GameObject droppedObject, Slot slot) 
    {
        OnDragCollectEnd?.Invoke(droppedObject, slot);
    }

    public void PublishDrag(GameObject draggedObject)
    {
        OnDrag?.Invoke(draggedObject);
    }
}