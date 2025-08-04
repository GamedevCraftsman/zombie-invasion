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

    [Header("Pools Parents")] 
    [SerializeField] private Transform enemyPoolParent;
    [SerializeField] private Transform effectsPoolParent;

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

        #region Enemy Pool

        Container.Bind<IPoolable<EnemyController>>()
            .To<EnemyPoolHandler>()
            .AsSingle();

        Container.Bind<IPool<EnemyController>>()
            .FromMethod(CreateEnemyPool)
            .AsSingle();

        #endregion

        #region Effects Pool

        Container.Bind<IPoolable<EnemyDeathEffectsService>>()
            .To<EnemyDeathEffectPoolHandler>()
            .AsSingle().NonLazy();

        Container.Bind<IPool<EnemyDeathEffectsService>>()
            .FromMethod(CreateDeathEffectsPool)
            .AsSingle().NonLazy();
        
        #endregion
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

    private IPool<EnemyDeathEffectsService> CreateDeathEffectsPool(InjectContext context)
    {
        var poolable = context.Container.Resolve<IPoolable<EnemyDeathEffectsService>>();
        var settings = context.Container.Resolve<EnemySpawnSettings>();
        Transform parent = effectsPoolParent;

        if (parent == null)
        {
            var poolRoot = new GameObject("Effects Pool");
            parent = poolRoot.transform;
        }

        return new Pool<EnemyDeathEffectsService>(poolable, settings.EffectsPoolSize, parent);
    }
}