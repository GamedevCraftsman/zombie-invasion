using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyManager : MonoBehaviour, IEnemyManager
{
    private EnemySpawnSettings _settings;
    private IPool<EnemyController> _enemyPool;
    private EnemySpawnController _spawnController;

    private int _nextSpawnIndex = 0;
    private int _enemiesLeft;

    [Inject]
    public void Construct(EnemySpawnSettings settings, IPool<EnemyController> enemyPool,
        EnemySpawnController spawnController)
    {
        _settings = settings;
        _enemyPool = enemyPool;
        _spawnController = spawnController;
    }

    #region For events

    public void CountEnemiesLeft()
    {
        Debug.LogWarning("CountEnemiesLeft");
        
        _enemiesLeft = _settings.TotalEnemyCount - _settings.EnemyPoolInitialSize;

        if (_enemiesLeft != 0)
        {
            _nextSpawnIndex = _settings.EnemyPoolInitialSize;
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
        
        _enemyPool.Get();
        enemy.transform.position = spawnPoints[_nextSpawnIndex];
    }

    private void DeactivateEnemy(EnemyController enemy)
    {
        enemy.OnEnemyDied -= HandleEnemyDeath;
        _enemyPool.Release(enemy);
    }
}