using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public class EnemyManager : BaseManager
{
    [Inject] private EnemySpawnSettings _settings;
    [Inject] private IPool<EnemyController> _enemyPool;
    [Inject] private EnemySpawnController _spawnController;

    //private Queue<int> _availableSpawnIndices = new Queue<int>();
    private HashSet<EnemyController> _activeEnemies = new HashSet<EnemyController>();
    private int _nextSpawnIndex;
    private int _enemiesLeft;
    
    protected override Task Initialize()
    {
        try
        {
            //InitializeSpawnQueue();
            SubscribeToEvents();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return Task.CompletedTask;
    }

    /*private void InitializeSpawnQueue()
    {
        // _enemiesLeft = _settings.TotalEnemyCount - _settings.EnemyPoolInitialSize;
        //
        // if (_enemiesLeft != 0)
        // {
        //     _nextSpawnIndex = _settings.EnemyPoolInitialSize + 1;
        // }
        // _availableSpawnIndices.Clear();

        // for (int i = _settings.TotalEnemyCount; i < _settings.SpawnPointCount; i++)
        // {
        //     _availableSpawnIndices.Enqueue(i);
        // }
    }*/

    private void CountEnemiesLeft()
    {
        _enemiesLeft = _settings.TotalEnemyCount - _settings.EnemyPoolInitialSize;
        
        if (_enemiesLeft != 0)
        {
            _nextSpawnIndex = _settings.EnemyPoolInitialSize + 1;
        }
    }
    
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<StartGameEvent>(OnGameStart);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus?.Subscribe<CarReachedEndEvent>(OnReachedEndOfGame);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<CarReachedEndEvent>(OnReachedEndOfGame);
        EventBus?.Unsubscribe<StartGameEvent>(OnGameStart);
        EventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
    }


    private void OnGameStart(StartGameEvent startEvent)
    {
        SubscribeToActiveEnemies();
        CountEnemiesLeft();
    }

    private void SubscribeToActiveEnemies()
    {
        var activeEnemies = FindObjectsOfType<EnemyController>();

        foreach (var enemy in activeEnemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                enemy.OnEnemyDied += HandleEnemyDeath;
                _activeEnemies.Add(enemy);
            }
        }
    }

    private void HandleEnemyDeath(EnemyController deadEnemy)
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

    private async void RespawnEnemy(EnemyController enemy)
    {
       //_availableSpawnIndices.Dequeue();
        var spawnPoints = _spawnController.AllSpawnPoints;

        if (_nextSpawnIndex < spawnPoints.Count && enemy != null)
        {
            enemy.transform.position = spawnPoints[_nextSpawnIndex];
            enemy.ResetForPooling();

            await enemy.InitializeAsync();

            Debug.Log($"Respawned enemy at spawn point {_nextSpawnIndex}");
            _nextSpawnIndex++;
        }
        else
        {
            //InitializeSpawnQueue();
            //_enemiesLeft = 0;
            DeactivateEnemy(enemy);
        }
    }

    private void DeactivateEnemy(EnemyController enemy)
    {
        enemy.OnEnemyDied -= HandleEnemyDeath;
        _activeEnemies.Remove(enemy);
        _enemyPool.Release(enemy);
    }

    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        OnGameEnd();
    }

    private void OnReachedEndOfGame(CarReachedEndEvent carReachedEndEvent)
    {
        OnGameEnd();
    }

    private void OnGameEnd()
    {
        _enemyPool.ReleaseAll();

        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDied -= HandleEnemyDeath;
            }
        }

        _activeEnemies.Clear();
        //_availableSpawnIndices.Clear();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();

        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDied -= HandleEnemyDeath;
            }
        }
    }
}