using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("Settings")]
    [SerializeField] private EnemySpawnSettings enemySpawnSettings;
    [SerializeField] private EnemySettings enemySettings;
    [SerializeField] private CarSettings carSettings;
    [SerializeField] private GameSettings gameSettings;
    
    [Header("Managers")]
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private GameManager gameManager;
    
    [Header("Pool Parent")]
    [SerializeField] private Transform enemyPoolParent;
    
    public override void InstallBindings()
    {
        // Managers
        Container.Bind<IUIManager>().To<UIManager>().AsSingle();
        
        Container.Bind<ICameraManager>().FromInstance(cameraManager).AsSingle();
        Container.Bind<IGameManager>().FromInstance(gameManager).AsSingle();
        Container.Bind<CameraController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IEventBus>().To<EventBus>().AsSingle();
        Container.Bind<HPManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SpawnMapManager>().FromComponentInHierarchy().AsSingle();
        
        // Enemy System Manager
        Container.Bind<EnemyManager>()
            .FromComponentInHierarchy()
            .AsSingle();
        
        //Controllers
        Container.Bind<CarController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<InputController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IInputController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<HPUIController>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<ITurretController>()
            .To<TurretController>()
            .FromComponentInHierarchy()
            .AsSingle();
        
        // Enemy Spawn Controller
        Container.Bind<EnemySpawnController>()
            .FromComponentInHierarchy()
            .AsSingle();
        
        // Pool System
        Container.Bind<IPoolable<EnemyController>>()
            .To<EnemyPoolHandler>()
            .AsSingle();
        
        Container.Bind<IPool<EnemyController>>()
            .FromMethod(CreateEnemyPool)
            .AsSingle();
        
        // Settings
        Container.Bind<EnemySpawnSettings>()
            .FromInstance(enemySpawnSettings)
            .AsSingle();

        Container.Bind<EnemySettings>().FromInstance(enemySettings).AsSingle();
        Container.Bind<GameSettings>().FromInstance(gameSettings);
        Container.BindInstance(carSettings).AsSingle();
    }
    
    private IPool<EnemyController> CreateEnemyPool(InjectContext context)
    {
        var poolable = context.Container.Resolve<IPoolable<EnemyController>>();
        var settings = context.Container.Resolve<EnemySpawnSettings>();
        Transform parent = enemyPoolParent;
        
        if (parent == null)
        {
            var poolRoot = new GameObject("Enemy Pool");
            parent = poolRoot.transform;
        }
        
        return new Pool<EnemyController>(poolable, settings.EnemyPoolInitialSize, parent);
    }
}