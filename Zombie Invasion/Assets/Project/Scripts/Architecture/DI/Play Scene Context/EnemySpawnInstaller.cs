using UnityEngine;
using Zenject;

public class EnemySpawnInstaller : MonoInstaller
{
    [Header("Settings")] 
    [SerializeField] private EnemySpawnSettings enemySpawnSettings;

    [Header("Managers")] 
    [SerializeField] private EnemyManager enemyManager;

    [Header("Controllers")] 
    [SerializeField] private EnemySpawnController enemySpawnController;

    [Header("Pool Parent")] 
    [SerializeField] private Transform enemyPoolParent;

    public override void InstallBindings()
    {
        //Settings
        Container.Bind<EnemySpawnSettings>().FromInstance(enemySpawnSettings).AsSingle();

        //Managers
        Container.Bind<IEnemyManager>().FromInstance(enemyManager).AsSingle();

        //Controllers
        Container.BindInterfacesTo<EnemyEventSubscriber>().AsSingle().NonLazy();
        Container.Bind<EnemySpawnController>().FromInstance(enemySpawnController).AsSingle().NonLazy();

        // Pool System
        Container.Bind<IPoolable<EnemyController>>()
            .To<EnemyPoolHandler>()
            .AsSingle();

        Container.Bind<IPool<EnemyController>>()
            .FromMethod(CreateEnemyPool)
            .AsSingle();

        //Services
        Container.Bind<ISpawnPointValidator>()
            .To<SpawnPointValidator>()
            .AsSingle();
        Container.Bind<ISpawnPointGenerator>()
            .To<SpawnPointGenerator>()
            .AsSingle();
        Container.Bind<IEnemySpawner>()
            .To<EnemySpawner>()
            .AsSingle();
        Container.Bind<IInitializable>()
            .To<EnemySpawnEventHandler>()
            .AsSingle();
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