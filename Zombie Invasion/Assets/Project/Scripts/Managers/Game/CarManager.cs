using System;
using System.Threading.Tasks;
using Zenject;
using UnityEngine;

public class CarManager : BaseManager
{
    private ICarController _carController;
    
    [Inject]
    public void Construct(ICarController carController)
    {
        _carController = carController;
    }
    
    protected override Task Initialize()
    {
        try
        {
            SubscribeToEvents();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
        return Task.CompletedTask;
    }
    
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<StartGameEvent>(OnGameStarted);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus.Subscribe<CarReachedEndEvent>(OnReachedEndGame);
        EventBus.Subscribe<ContinueGameEvent>(OnContinueGame);
        EventBus.Subscribe<RestarGameEvent>(OnRestartGame);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<StartGameEvent>(OnGameStarted);
        EventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus?.Unsubscribe<CarReachedEndEvent>(OnReachedEndGame);
        EventBus?.Unsubscribe<ContinueGameEvent>(OnContinueGame);
        EventBus?.Unsubscribe<RestarGameEvent>(OnRestartGame);
    }
    
    private void OnGameStarted(StartGameEvent startEvent)
    {
        _carController.StartMovement();
    }

    private void OnRestartGame(RestarGameEvent restartEvent)
    {
        _carController.ResetPosition();
    }

    private void OnReachedEndGame(CarReachedEndEvent carReachedEndEvent)
    {
        _carController.StopMovement();
    }

    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        _carController.StopMovement();
    }

    private void OnContinueGame(ContinueGameEvent continueEvent)
    {
        _carController.ResetCarState();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}