using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameSettings _gameSettings;
    [SerializeField] private DraggableObject _draggableObjectPrefab;

    public override void InstallBindings()
    {
        Container.Bind<EventBus>().AsSingle().NonLazy();
        Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle().NonLazy();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<InputManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<UIManager>().FromComponentInHierarchy().AsSingle();
        
        Container.BindMemoryPool<DraggableObject, DraggableObjectPool>()
            .WithInitialSize(20)
            .FromComponentInNewPrefab(_draggableObjectPrefab)
            .UnderTransformGroup("DraggableObjects");
    }
}