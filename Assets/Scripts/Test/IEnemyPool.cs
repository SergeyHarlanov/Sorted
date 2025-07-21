using UnityEngine;

public interface IEnemyPool
{
    DraggableObject Get(Vector3 position);
    void Return(DraggableObject enemy);
}