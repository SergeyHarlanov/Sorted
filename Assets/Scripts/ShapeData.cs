// Файл: ShapeData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New ShapeData", menuName = "Shape Data")]
public class ShapeData : ScriptableObject
{
    public ShapeType shapeType;
    public Sprite sprite;
}   
// Файл: ShapeType.cs
public enum ShapeType
{
    Square,
    Circle,
    Triangle,
    Star
}