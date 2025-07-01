using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class TurretManager : BaseManager
{
    [Inject] private WeaponSettings _weaponSettings;
    [Inject] private ITurretController _turretController;  
    [Inject] private IInputController _inputController;
    [Inject] private IFireController _fireController;

    private ITurretInputHandler _inputHandler;
    protected override Task Initialize()
    {
        try
        {
            _inputHandler = new TurretInputHandler(_turretController, _weaponSettings, _inputController,_fireController);
            
            SubscribeToEvents();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return Task.CompletedTask;
    }
    
    #region Set up Events
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<StartGameEvent>(OnGameStarted);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus.Subscribe<CarReachedEndEvent>(OnCarReached);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<StartGameEvent>(OnGameStarted);
        EventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus?.Unsubscribe<CarReachedEndEvent>(OnCarReached);
    }

    private void OnGameStarted(StartGameEvent gameStartedEvent)
    {
        _turretController.EnableControl();
    }

    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        _turretController.DisableControl();
        _turretController.ResetRotation();
        
       //Add StopDragging() =>  _isDragging = false;
       _inputHandler.StopDragging();
    }

    private void OnCarReached(CarReachedEndEvent carReachedEndEvent)
    {
        _turretController.DisableControl();
        _turretController.ResetRotation();
        
        _inputHandler.StopDragging();
    }
    #endregion

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}