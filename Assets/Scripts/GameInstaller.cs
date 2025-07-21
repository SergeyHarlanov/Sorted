// Файл: GameInstaller.cs (дополнительная привязка)
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Tooltip("Перетащите сюда ваш ассет GameSettings ScriptableObject.")]
    [SerializeField] private GameSettings _gameSettings;

    public override void InstallBindings()
    {
        if (_gameSettings == null)
        {
            Debug.LogError("GameSettings ScriptableObject не назначен в GameInstaller!");
            return;
        }

        Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle().NonLazy();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        
        // 🔥 Привязываем UIManager, предполагая, что он является компонентом в иерархии вашей сцены 🔥
        Container.Bind<UIManager>().FromComponentInHierarchy().AsSingle();
        // Если UIManager создается динамически или как префаб, используйте FromNewComponent или FromComponentInNewPrefab
    }
}