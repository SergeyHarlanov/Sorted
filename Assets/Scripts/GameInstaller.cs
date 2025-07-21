using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameSettings _gameSettings;
    [SerializeField] private DraggableObject _draggableObjectPrefab; // Ссылка на префаб для пула

    public override void InstallBindings()
    {
        // Основные зависимости
        Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle().NonLazy();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<InputManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<UIManager>().FromComponentInHierarchy().AsSingle();
        
        // Настройка пул-фабрики (MemoryPool)
        Container.BindMemoryPool<DraggableObject, DraggableObjectPool>()
            .WithInitialSize(10) // Предварительно создать 10 объектов
            .FromComponentInNewPrefab(_draggableObjectPrefab) // Использовать этот префаб
            .UnderTransformGroup("DraggableObjects"); // Сложить все объекты в иерархии для порядка
    }
}