using System;
using UnityEngine;
using Zenject;

public class ProgressIncreaseService : IDisposable, IProgressIncreaseService
{
    private readonly IEventBus _eventBus;
    private readonly ProgressSettings _progressSettings;
    
    private MapLenghtIncrease _mapLenghtIncrease;
    private EnemiesCountIncrease _enemiesCountIncrease;
    private int _mapIncrease;
    private int _enemiesIncrease;
    private int _lvl = 1;

    public int MapIncrease => _mapIncrease;
    public int EnemiesIncrease => _enemiesIncrease;
    [Inject]
    public ProgressIncreaseService(IEventBus eventBus, ProgressSettings progressSettings)
    {
        _eventBus = eventBus;
        _progressSettings = progressSettings;
        
        Subscribe();
        Init();
    }

    private void Init()
    {
        _mapLenghtIncrease = new MapLenghtIncrease(_progressSettings);
        _enemiesCountIncrease = new EnemiesCountIncrease(_progressSettings);
    }
    
    private void Subscribe()
    {
        _eventBus.Subscribe<ContinueGameEvent>(OnGameContinue);
    }

    private void Unsubscribe()
    {
        _eventBus?.Unsubscribe<ContinueGameEvent>(OnGameContinue);
    }

    private void OnGameContinue(ContinueGameEvent gameEvent)
    {
        _lvl++;
        
        IncreaseMapLenght();
        _enemiesCountIncrease.EnemiesIncrease(ref _enemiesIncrease);
    }

    private void IncreaseMapLenght()
    {
        if (_lvl % 5 == 0)
        {
            _mapLenghtIncrease.IncreaseMapLenght(ref _mapIncrease);
        }
    }
    
    public void Dispose()
    {
        Unsubscribe();
    
        Debug.LogWarning("Progress service: Unsubscribe called");
    }
}