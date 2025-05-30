using UnityEngine;
using UnityEngine.Rendering;
using Zenject;
public class GameInstaller : MonoInstaller
{
    [SerializeField] private CarSettings carSettings;
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private CameraManager cameraManager;
    
    public override void InstallBindings()
    {
        // Managers
        Container.Bind<IUIManager>().To<UIManager>().AsSingle();
        Container.Bind<IEnemyManager>().To<EnemyManager>().AsSingle();
        
        Container.Bind<ICameraManager>().FromInstance(cameraManager).AsSingle();
        Container.Bind<CameraController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IEventBus>().To<EventBus>().AsSingle();
        Container.Bind<IGameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<HPManager>().FromComponentInHierarchy().AsSingle();
        
        //Controllers
        Container.Bind<CarController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<InputController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IInputController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<HPUIController>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<ITurretController>()
            .To<TurretController>()
            .FromComponentInHierarchy()
            .AsSingle();
        
        // Settings
        Container.Bind<GameSettings>().FromInstance(gameSettings);
        Container.BindInstance(carSettings).AsSingle();
        
    }
}