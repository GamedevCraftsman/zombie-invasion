using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyEventSubscriber : IInitializable, IDisposable
{
    //General
    private readonly IEventBus _eventBus;

    //EnemySpawn
    private readonly IEnemyManager _enemyManager;
    private readonly IPool<EnemyController> _enemyPool;
    
    private List<EnemyController> _activeEnemies = new();
    [Inject]
    public EnemyEventSubscriber(IEventBus eventBus, IEnemyManager enemyManager, IPool<EnemyController> enemyPool)
    {
        _eventBus = eventBus;
        _enemyManager = enemyManager;
        _enemyPool = enemyPool;
    }

    public void Initialize()
    {
        SubscribeToEvents();
    }

    #region Events
    
    private void SubscribeToEvents()
    {
        _eventBus.Subscribe<StartGameEvent>(OnGameStart);
        _eventBus.Subscribe<GameOverEvent>(OnGameOver);
        _eventBus?.Subscribe<CarReachedEndEvent>(OnReachedEndOfGame);
    }

    private void UnsubscribeFromEvents()
    {
        _eventBus?.Unsubscribe<StartGameEvent>(OnGameStart);
        _eventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
        _eventBus?.Unsubscribe<CarReachedEndEvent>(OnReachedEndOfGame);
    }

    private void OnGameStart(StartGameEvent startEvent)
    {
        Debug.LogWarning("Start Game");
        _activeEnemies = _enemyPool.AllItems;
        _enemyManager.CountEnemiesLeft();
        
        SubscribeToActiveEnemies();
    }

    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        OnGameEnd();
    }

    private void OnReachedEndOfGame(CarReachedEndEvent carReachedEndEvent)
    {
        OnGameEnd();
    }
    
    #endregion
    
    public void Dispose()
    {
        UnsubscribeFromEvents();
        UnsubscribeFromActiveEnemies();
    }
    
    private void SubscribeToActiveEnemies()
    {
        foreach (var enemy in _activeEnemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                enemy.OnEnemyDied += _enemyManager.HandleEnemyDeath;
            }
        }
    }

    private void UnsubscribeFromActiveEnemies()
    {
        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDied -= _enemyManager.HandleEnemyDeath;
            }
        }
    }
    private void OnGameEnd()
    {
        _enemyPool.ReleaseAll();
        UnsubscribeFromActiveEnemies();
    }
}