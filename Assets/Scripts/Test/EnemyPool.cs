using UnityEngine;
using Zenject;
    /*
public class EnemyPool : MemoryPool<ShapeType, DraggableObject>
{
    private readonly Transform _poolParent;
    private readonly DraggableObject _basicEnemyPrefab;
    private readonly DraggableObject _bossEnemyPrefab;

    public EnemyPool(Transform poolParent, DraggableObject basic, DraggableObject boss)
    {
        _poolParent = poolParent;
        _basicEnemyPrefab = basic;
        _bossEnemyPrefab = boss;
    }

    protected override void Reinitialize(ShapeType type, DraggableObject item)
    {
        item.Type = type;
    }

    protected override void OnCreated(DraggableObject item)
    {
        DraggableObject prefab = type == ShapeType.Circle ? _basicEnemyPrefab : _bossEnemyPrefab;
        DraggableObject enemy = GameObject.Instantiate(prefab, _poolParent);
        enemy.ShapeData.shapeType = type;
        return enemy;
    }

    protected override void OnDespawned(DraggableObject item)
    {
       // item.OnDespawn();
        base.OnDespawned(item);
    }

    protected override void OnSpawned(DraggableObject item)
    {
        base.OnSpawned(item);
    }

    protected override void OnSpawned(ShapeType p1, DraggableObject p2)
    {
      //  p2.transform.position = Vector3.zero; // Можно передавать позицию
     //   p2.OnSpawn();
    }
}
*/