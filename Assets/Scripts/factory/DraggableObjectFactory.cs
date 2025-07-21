using UnityEngine;
using Zenject;

public class DraggableObjectFactory
{
    private readonly DiContainer _container;

    // Zenject automatically injects the container into the factory's constructor.
    public DraggableObjectFactory(DiContainer container)
    {
        _container = container;
    }

    /// <summary>
    /// Creates a new draggable object, instantiates it using the Zenject container,
    /// and initializes it with the required data.
    /// </summary>
    /// <returns>The fully initialized DraggableObject instance.</returns>
    public DraggableObject Create(DraggableObject prefab, ShapeData shapeData, Vector3 startPos, Vector3 endPos, float moveSpeed, GameManager gameManager, InputManager inputManager)
    {
        // Use the container to instantiate the prefab. This is the correct way in Zenject.
        DraggableObject newObject = _container.InstantiatePrefabForComponent<DraggableObject>(prefab, startPos, Quaternion.identity, null);

        // After creation, pass all necessary data to the object's Initialize method.
        newObject.Initialize(shapeData, startPos, endPos, moveSpeed, gameManager, inputManager);

        return newObject;
    }
}