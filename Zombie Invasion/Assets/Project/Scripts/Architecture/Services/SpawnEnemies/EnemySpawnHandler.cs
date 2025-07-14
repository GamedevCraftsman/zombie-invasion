using UnityEngine;
using Zenject;

public class EnemySpawnEventHandler : IInitializable
{
    private readonly EnemySpawnController _controller;

    private readonly ISpawnPointGenerator _generator;
    private readonly IEnemySpawner _spawner;
    private readonly EnemySpawnSettings _settings;
    private readonly IPool<EnemyController> _pool;
    private readonly SpawnMapManager _mapManager;

    [Inject]
    public EnemySpawnEventHandler(EnemySpawnController controller, SpawnMapManager mapManager,
        EnemySpawnSettings settings,
        IPool<EnemyController> pool,
        ISpawnPointGenerator generator,
        IEnemySpawner spawner)
    {
        _controller = controller;
        
        _mapManager = mapManager;
        _settings = settings;
        _pool = pool;
        _generator = generator;
        _spawner = spawner;
    }

    public void Initialize()
    {
        Debug.LogWarning("Initialize");
        _controller.Inject(_mapManager, _settings, _pool, _generator, _spawner);
    }
}