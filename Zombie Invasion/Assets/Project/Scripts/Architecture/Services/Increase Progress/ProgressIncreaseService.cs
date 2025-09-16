using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ProgressIncreaseService : IDisposable, IProgressIncreaseService
{
    private readonly IEventBus _eventBus;
    private readonly IProgressUIUpdateService _progressUIUpdateService;
    private readonly ProgressSettings _progressSettings;

    private MapLenghtIncrease _mapLenghtIncrease;
    private EnemiesCountIncrease _enemiesCountIncrease;
    private int _mapIncrease;
    private int _enemiesIncrease;
    private int _lvl = 1;

    public int MapIncrease => _mapIncrease;
    public int EnemiesIncrease => _enemiesIncrease;

    [Inject]
    public ProgressIncreaseService(IEventBus eventBus, ProgressSettings progressSettings,
        IProgressUIUpdateService progressUIUpdateService)
    {
        _eventBus = eventBus;
        _progressSettings = progressSettings;
        _progressUIUpdateService = progressUIUpdateService;

        Subscribe();
        Init();
    }

    private void Init()
    {
        _mapLenghtIncrease = new MapLenghtIncrease(_progressSettings);
        _enemiesCountIncrease = new EnemiesCountIncrease(_progressSettings);

        _progressUIUpdateService.ChangeLevelText(_lvl.ToString());
    }

    #region Events

    private void Subscribe()
    {
        _eventBus.Subscribe<ContinueGameEvent>(OnGameContinue);
    }

    private void Unsubscribe()
    {
        _eventBus?.Unsubscribe<ContinueGameEvent>(OnGameContinue);
    }

    private  void OnGameContinue(ContinueGameEvent gameEvent)
    {
        IncreaseLvlProperties();
    }

    #endregion

    private void IncreaseLvlProperties()
    {
        _lvl++;

        _progressUIUpdateService.ChangeLevelText(_lvl.ToString());
        _enemiesIncrease = _enemiesCountIncrease.EnemiesIncrease(_enemiesIncrease);
        IncreaseMapLenght();
    }

    private void IncreaseMapLenght()
    {
        if (_lvl % _progressSettings.ActionInterval == 0)
        {
            _mapIncrease = _mapLenghtIncrease.IncreaseMapLenght(_mapIncrease);
        }
    }

    public void Dispose()
    {
        Unsubscribe();
    }
}