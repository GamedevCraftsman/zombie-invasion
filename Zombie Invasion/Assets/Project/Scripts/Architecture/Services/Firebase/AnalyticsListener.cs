using System;
using Firebase.Analytics;
using UnityEngine;
using Zenject;

public class AnalyticsListener : IDisposable
{
    private readonly IEventBus _eventBus;
    private readonly DataManageService _dataManageService;
    
    [Inject]
    public AnalyticsListener(IEventBus eventBus, DataManageService dataManageService)
    {
        _eventBus = eventBus;
        _dataManageService = dataManageService;
        
        SetUpListener();
    }

    private void SetUpListener()
    {
        Subscribe();
    }
    
    private void Subscribe()
    {
        _eventBus.Subscribe<StartGameEvent>(OnGameStarted);
    }

    private void Unsubscribe()
    {
        _eventBus.Unsubscribe<StartGameEvent>(OnGameStarted);
    }

    #region Events

    private void OnGameStarted(StartGameEvent startGameEvent)
    {
       FirebaseAnalytics.LogEvent(
            "player_level",
            new Parameter("level", _dataManageService.LocalLvlDB.LvlNumber)
        );
    }

    #endregion

    public void Dispose()
    {
        Unsubscribe();
        
        Debug.LogWarning("AnalyticsListener: Disposed");
    }
}