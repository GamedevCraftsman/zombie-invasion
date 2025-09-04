using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ProgressIncreaseService : IDisposable, IProgressIncreaseService
{
    private readonly IEventBus _eventBus;
    private readonly IProgressUIUpdateService _progressUIUpdateService;
    private readonly ProgressSettings _progressSettings;
    private readonly DataManageService _dataManageService;

    private MapLenghtIncrease _mapLenghtIncrease;
    private EnemiesCountIncrease _enemiesCountIncrease;
    private int _mapIncrease;
    private int _enemiesIncrease;
    private int _lvl = 1;

    public int MapIncrease => _mapIncrease;
    public int EnemiesIncrease => _enemiesIncrease;

    [Inject]
    public ProgressIncreaseService(IEventBus eventBus, ProgressSettings progressSettings,
        IProgressUIUpdateService progressUIUpdateService, DataManageService dataManageService)
    {
        _eventBus = eventBus;
        _progressSettings = progressSettings;
        _progressUIUpdateService = progressUIUpdateService;
        _dataManageService = dataManageService;
        
        Subscribe();
        Init();
    }

    private void Init()
    {
        //Set _mapIncrease & _enemiesIncrease from LocalLvlDB (DataMangeService)
        _mapIncrease = _dataManageService.LocalLvlDB.LvlLenghtIncrease;
        _enemiesIncrease = _dataManageService.LocalLvlDB.EnemyCountIncrease;
        
        _mapLenghtIncrease = new MapLenghtIncrease(_progressSettings);
        _enemiesCountIncrease = new EnemiesCountIncrease(_progressSettings);

        //Set lvl. _lvl = LocalLvlDB.lvl;
        _lvl = _dataManageService.LocalLvlDB.LvlNumber;
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

    private async void OnGameContinue(ContinueGameEvent gameEvent)
    {
        try
        {
            await IncreaseLvlProperties();
        }
        catch (Exception e)
        {
            Debug.LogError($"IncreaseLvlProperties error: {e.Message}");
        }
    }

    #endregion

    private async Task IncreaseLvlProperties()
    {
        _lvl++;

        _progressUIUpdateService.ChangeLevelText(_lvl.ToString());
        _enemiesIncrease = _enemiesCountIncrease.EnemiesIncrease(_enemiesIncrease);
        await IncreaseMapLenght();
        
        //Save lvl & enemy increase
        await _dataManageService.SaveData(lvl: _lvl,
            enemyIncrease: _enemiesIncrease);
    }

    private async Task IncreaseMapLenght()
    {
        if (_lvl % _progressSettings.ActionInterval == 0)
        {
            _mapIncrease = _mapLenghtIncrease.IncreaseMapLenght(_mapIncrease);
            
            //Save _mapiIncrease
            await _dataManageService.SaveData(lvlLengthIncrease: _mapIncrease);
        }
    }
    
    public void Dispose()
    {
        Unsubscribe();
    }
}