using System;
using UnityEngine;
using Zenject;

public class MainScreenEventSubscriber : IDisposable
{
    private readonly IEventBus _eventBus;
    private readonly UISettings _uiSettings;
    private readonly IMainScreenUIChanger _mainScreenUIChanger;
    
    [Inject]
    public MainScreenEventSubscriber(IEventBus eventBus, IMainScreenUIChanger mainScreenUIChanger, UISettings uiSettings)
    {
        _eventBus = eventBus;
        _mainScreenUIChanger = mainScreenUIChanger;
        _uiSettings = uiSettings;
        
        Subscribe();
    }

    private void Subscribe()
    {
        _eventBus.Subscribe<ReadyGameEvent>(OnGameReady);
        _eventBus.Subscribe<RestarGameEvent>(OnGameRestart);
        _eventBus.Subscribe<ContinueGameEvent>(OnGameContinue);
    }

    private void Unsubscribe()
    {
        _eventBus?.Unsubscribe<ReadyGameEvent>(OnGameReady);
        _eventBus?.Unsubscribe<RestarGameEvent>(OnGameRestart);
        _eventBus?.Unsubscribe<ContinueGameEvent>(OnGameContinue);
    }

    private void OnGameReady(ReadyGameEvent readyGameEvent)
    {
        _mainScreenUIChanger.CloseMainMenu(_uiSettings);
    }

    private void OnGameRestart(RestarGameEvent restartGameEvent)
    {
        _mainScreenUIChanger.OpenMainMenu(_uiSettings);
    }

    private void OnGameContinue(ContinueGameEvent continueGameEvent)
    {
        _mainScreenUIChanger.OpenMainMenu(_uiSettings);
    }
    
    public void Dispose()
    {
        Unsubscribe();
        
        Debug.LogWarning("MainScreenEventSubscriber unsubscribe called");
    }
}