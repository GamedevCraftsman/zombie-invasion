using System;
using System.Threading.Tasks;
using Project.Scripts.Controllers;
using UnityEngine;
using Zenject;

public class GameManager : BaseManager, IGameManager
{
    // Dependencies
    private HPManager _hpManager;
    private InputController _inputController;

    // State
    private GameState _currentState = GameState.Menu;

    // Properties
    public GameState CurrentState => _currentState;

    [Inject]
    public void Construct(HPManager hpManager, InputController inputController)
    {
        _hpManager = hpManager;
        _inputController = inputController;
    }

    protected override Task Initialize()
    {
        try
        {
            SubscribeToEvents();
            ChangeState(GameState.Menu);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return Task.CompletedTask;
    }

    #region Events

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<StartGameEvent>(OnStartGameEvent);
        EventBus.Subscribe<RestarGameEvent>(OnGameRestart);
        EventBus.Subscribe<CarReachedEndEvent>(OnCarReachedEnd);
        EventBus.Subscribe<ContinueGameEvent>(OnContinueGame);
        EventBus.Subscribe<GameOverEvent>(OnGameOverEvent);
        EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<StartGameEvent>(OnStartGameEvent);
        EventBus?.Unsubscribe<RestarGameEvent>(OnGameRestart);
        EventBus?.Unsubscribe<CarReachedEndEvent>(OnCarReachedEnd);
        EventBus?.Unsubscribe<ContinueGameEvent>(OnContinueGame);
        EventBus?.Unsubscribe<GameOverEvent>(OnGameOverEvent);
        EventBus?.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }

    private void OnStartGameEvent(StartGameEvent startEvent)
    {
        if (_currentState == GameState.Menu)
        {
            StartGame();
        }
    }

    private void OnCarReachedEnd(CarReachedEndEvent carEndEvent)
    {
        if (_hpManager.IsAlive)
        {
            EndGame(true);
        }
    }

    private void OnGameOverEvent(GameOverEvent gameOverEvent)
    {
        ChangeState(GameState.GameOver);
    }

    private void OnContinueGame(ContinueGameEvent continueGameEvent)
    {
        ChangeState(GameState.Menu);
    }

    private void OnPlayerDamaged(PlayerDamagedEvent damageEvent)
    {
        if (!_hpManager.IsAlive && _currentState == GameState.Playing)
        {
            EndGame(false);
        }
    }

    private void ChangeState(GameState newState)
    {
        if (_currentState == newState) return;

        GameState previousState = _currentState;
        _currentState = newState;
    }

    private void OnGameRestart(RestarGameEvent gameRestartEvent)
    {
        ChangeState(GameState.Menu);
    }

    #endregion

    #region IGameManager
    public void StartGame()
    {
        if (_currentState != GameState.Menu)
        {
            return;
        }

        ResetGameState();

        ChangeState(GameState.Playing);
    }

    public void EndGame(bool victory)
    {
        if (_currentState != GameState.Playing)
        {
            return;
        }

        ChangeState(victory ? GameState.Victory : GameState.GameOver);

        if (victory)
            EventBus.Fire(new CarReachedEndEvent());
        else
            EventBus.Fire(new GameOverEvent());
    }

    public void RestartGame()
    {
        if (_currentState == GameState.Playing)
        {
            return;
        }

        ResetGameState();

        ChangeState(GameState.Menu);
    }

    #endregion
    
    private void ResetGameState()
    {
        _inputController.ResetForNewGame();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}