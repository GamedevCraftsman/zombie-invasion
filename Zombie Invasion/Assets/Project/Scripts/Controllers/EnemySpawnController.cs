using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemySpawnController : BaseController
{
    private IProgressIncreaseService _progressService;
    private ISpawnPointGenerator _generator;
    private IEnemySpawner _spawner;
    private EnemySpawnSettings _settings;
    private IPool<EnemyController> _pool;
    private SpawnMapManager _mapManager;

    private List<Vector3> _spawnPoints;
    public List<Vector3> AllSpawnPoints => _spawnPoints;
    private int _spawnEnemyCount;
    
    public void Inject(
        SpawnMapManager mapManager,
        EnemySpawnSettings settings,
        IPool<EnemyController> pool,
        ISpawnPointGenerator generator,
        IEnemySpawner spawner,
        IProgressIncreaseService progressService)
    {
        _mapManager = mapManager;
        _settings = settings;
        _pool = pool;
        _generator = generator;
        _spawner = spawner;
        _progressService = progressService;
    }

    protected override Task Initialize()
    {
        try
        {
            _spawnPoints = new List<Vector3>(capacity: _spawnEnemyCount);
            SubscribeToEvents();
            GenerateSpawnPoints();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return Task.CompletedTask;
    }

    #region Events

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<ReadyGameEvent>(OnGameReady);
        EventBus.Subscribe<RestarGameEvent>(OnGameRestart);
        EventBus.Subscribe<ContinueGameEvent>(OnContinueGame);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus.Unsubscribe<ReadyGameEvent>(OnGameReady);
        EventBus.Unsubscribe<RestarGameEvent>(OnGameRestart);
        EventBus.Unsubscribe<ContinueGameEvent>(OnContinueGame);
    }

    private void OnGameReady(ReadyGameEvent e)
    {
        SpawnEnemies();
    }

    private void OnGameRestart(RestarGameEvent e)
    {
        GenerateSpawnPoints();
    }

    private void OnContinueGame(ContinueGameEvent e)
    {
        GenerateSpawnPoints();
    }

    #endregion

    private void GenerateSpawnPoints()
    {
        _spawnEnemyCount = _settings.TotalEnemyCount + _progressService.EnemiesIncrease;
        
        var tiles = _mapManager.GroundTiles;
        _spawnPoints = _generator.GeneratePoints(tiles);
    }

    private void SpawnEnemies()
    {
        _spawner.SpawnEnemies(_spawnPoints, _pool, _spawnEnemyCount, SetMinPoolSize());
    }

    public int SetMinPoolSize()
    {
        if (_settings.EnemyPoolInitialSize < _settings.MinPoolSize(_spawnEnemyCount, _progressService.MapIncrease))
        {
            return _settings.MinPoolSize(_spawnEnemyCount, _progressService.MapIncrease);
        }

        return _settings.EnemyPoolInitialSize;
    }
    
    public void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}