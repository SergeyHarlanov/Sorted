using UnityEngine;

/// <summary>
/// Класс-контейнер для передачи параметров при спавне объекта из пула.
/// </summary>
public class DraggableObjectSpawnParams
{
    public ShapeData ShapeData { get; set; }
    public Vector3 StartPos { get; set; }
    public Vector3 EndPos { get; set; }
    public float MoveSpeed { get; set; }
    public GameManager GameManager { get; set; }
    public InputManager InputManager { get; set; }
}