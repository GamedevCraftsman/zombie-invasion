using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyManager : MonoBehaviour, IEnemyManager
{
    private IProgressIncreaseService _progressService;
    private EnemySpawnSettings _settings;
    private IPool<EnemyController> _enemyPool;
    private EnemySpawnController _spawnController;

    private int _nextSpawnIndex = 0;
    private int _enemiesLeft;

    [Inject]
    public void Construct(EnemySpawnSettings settings, IPool<EnemyController> enemyPool,
        EnemySpawnController spawnController, IProgressIncreaseService progressIncreaseService)
    {
        _settings = settings;
        _enemyPool = enemyPool;
        _spawnController = spawnController;
        _progressService = progressIncreaseService;
    }

    #region For events

    public void CountEnemiesLeft()
    {
        _enemiesLeft = (_settings.TotalEnemyCount + _progressService.EnemiesIncrease) - _spawnController.SetMinPoolSize();

        if (_enemiesLeft != 0)
        {
            _nextSpawnIndex = _spawnController.SetMinPoolSize();
        }
    }
    
    #endregion

    public void HandleEnemyDeath(EnemyController deadEnemy)
    {
        if (_enemiesLeft > 0)
        {
            RespawnEnemy(deadEnemy);
        }
        else
        {
            DeactivateEnemy(deadEnemy);
        }
    }

    private void RespawnEnemy(EnemyController enemy)
    {
        List<Vector3> spawnPoints = _spawnController.AllSpawnPoints;
        
        Debug.LogWarning($"Points count: {_spawnController.AllSpawnPoints.Count}\nNext spawn index: {_nextSpawnIndex}\nEnemies Left: {_enemiesLeft}");

        if (_nextSpawnIndex < spawnPoints.Count && enemy != null)
        {
            Respawn(enemy, spawnPoints);

            Debug.Log($"Respawned enemy at spawn point {_nextSpawnIndex}");
            _nextSpawnIndex++;
        }
        else
        {
            DeactivateEnemy(enemy);
        }
    }

    private void Respawn(EnemyController enemy, List<Vector3> spawnPoints)
    {
        enemy.ResetForPooling();
        _enemyPool.Release(enemy);
        
        var enemyGet = _enemyPool.Get();
        enemyGet.transform.position = spawnPoints[_nextSpawnIndex];
    }

    private void DeactivateEnemy(EnemyController enemy)
    {
        enemy.OnEnemyDied -= HandleEnemyDeath;
        _enemyPool.Release(enemy);
    }
}