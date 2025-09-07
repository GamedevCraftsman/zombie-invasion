using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class TurretManager : BaseManager
{
    private WeaponSettings _weaponSettings;
    private ITurretController _turretController;  
    private IInputController _inputController;
    private IFireController _fireController;
    private IAimStateService _aimStateService;
    
    private ITurretInputHandler _inputHandler;

    [Inject]
    public void Construct(ITurretController turretController, IInputController inputController,
        IFireController fireController, WeaponSettings weaponSettings, IAimStateService aimStateService)
    {
        _weaponSettings = weaponSettings;
        _turretController = turretController;
        _inputController = inputController;
        _fireController = fireController;
        _aimStateService = aimStateService;
    }
    
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
        _aimStateService.AimManage(true);
    }

    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        _turretController.DisableControl();
        _turretController.ResetRotation();
        _aimStateService.AimManage(false);
        
       //Add StopDragging() =>  _isDragging = false;
       _inputHandler.StopDragging();
    }

    private void OnCarReached(CarReachedEndEvent carReachedEndEvent)
    {
        _turretController.DisableControl();
        _turretController.ResetRotation();
        _aimStateService.AimManage(false);
        
        _inputHandler.StopDragging();
    }
    #endregion

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}