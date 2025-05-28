using UnityEngine;
using Zenject;
public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private CameraManager cameraManager;
    
    public override void InstallBindings()
    {
        // Managers
        Container.Bind<IGameManager>().To<GameManager>().AsSingle();
    
        Container.Bind<IUIManager>().To<UIManager>().AsSingle();
        Container.Bind<IEnemyManager>().To<EnemyManager>().AsSingle();
        
        Container.Bind<ICameraManager>().FromInstance(cameraManager).AsSingle();
        Container.Bind<CameraController>().FromComponentInHierarchy().AsSingle();
        
        // Settings
        Container.Bind<GameSettings>().FromInstance(gameSettings);
        
    }
}