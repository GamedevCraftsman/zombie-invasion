using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class EnemyManager : BaseManager
{
    [Inject] private EnemySpawnSettings _settings;
    [Inject] private IPool<EnemyController> _enemyPool;
    [Inject] private EnemySpawnController _spawnController;

    private Queue<int> _availableSpawnIndices = new Queue<int>();
    private HashSet<EnemyController> _activeEnemies = new HashSet<EnemyController>();

    protected override Task Initialize()
    {
        try
        {
            InitializeSpawnQueue();
            SubscribeToEvents();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return Task.CompletedTask;
    }

    private void InitializeSpawnQueue()
    {
        _availableSpawnIndices.Clear();

        for (int i = _settings.TotalEnemyCount; i < _settings.SpawnPointCount; i++)
        {
            _availableSpawnIndices.Enqueue(i);
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
        if (_availableSpawnIndices.Count > 0)
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
        int nextSpawnIndex = _availableSpawnIndices.Dequeue();
        var spawnPoints = _spawnController.AllSpawnPoints;

        if (nextSpawnIndex < spawnPoints.Count && enemy != null)
        {
            enemy.transform.position = spawnPoints[nextSpawnIndex];
            enemy.ResetForPooling();

            await enemy.InitializeAsync();

            Debug.Log($"Respawned enemy at spawn point {nextSpawnIndex}");
        }
        else
        {
            DeactivateEnemy(enemy);
        }
    }

    private void DeactivateEnemy(EnemyController enemy)
    {
        enemy.OnEnemyDied -= HandleEnemyDeath;
        _activeEnemies.Remove(enemy);
        _enemyPool.Release(enemy);

        Debug.Log($"Deactivated enemy. Remaining active: {_activeEnemies.Count}");
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
        _availableSpawnIndices.Clear();

        Debug.Log("Game over - all enemies deactivated");
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