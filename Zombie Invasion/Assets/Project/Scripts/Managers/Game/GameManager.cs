using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GameManager : BaseManager, IGameManager
{
    // Dependencies
    [Inject] private CarController carController;
    [Inject] private HPManager hpManager;
    [Inject] private InputController inputController;
    [Inject] private CarHPUIController hpUIController;
    
    // State
    private GameState currentState = GameState.Menu;
    
    // Properties
    public GameState CurrentState => currentState;
    
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
        if (currentState == GameState.Menu)
        {
            StartGame();
        }
    }
    
    private void OnCarReachedEnd(CarReachedEndEvent carEndEvent)
    {
        if (hpManager.IsAlive)
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
        if (!hpManager.IsAlive && currentState == GameState.Playing)
        {
            EndGame(false);
        }
    }
    
    private void ChangeState(GameState newState)
    {
        if (currentState == newState) return;
        
        GameState previousState = currentState;
        currentState = newState;
        
        OnStateEntered(newState, previousState);
    }
    
    private void OnStateEntered(GameState newState, GameState previousState)
    {
        switch (newState)
        {
            case GameState.Menu:
                OnMenuEntered();
                break;
                
            case GameState.Playing:
                OnPlayingEntered();
                break;
                
            case GameState.GameOver:
                OnGameOverEntered();
                break;
                
            case GameState.Victory:
                OnVictoryEntered();
                break;
        }
    }
    
    private void OnMenuEntered()
    {
        Debug.Log("📋 Увійшли в стан Menu");
    }

    private void OnGameRestart(RestarGameEvent gameRestartEvent)
    {
        ChangeState(GameState.Menu);
    }
    
    private void OnPlayingEntered()
    {

    }
    
    private void OnGameOverEntered()
    {

    }
    
    private void OnVictoryEntered()
    {

    }
    
    public void StartGame()
    {
        if (currentState != GameState.Menu)
        {
            Debug.LogWarning($"Не можна запустити гру зі стану {currentState}!");
            return;
        }
        
        ResetGameState();

        ChangeState(GameState.Playing);
    }
    
    public void EndGame(bool victory)
    {
        if (currentState != GameState.Playing)
        {
            Debug.LogWarning($"Не можна завершити гру зі стану {currentState}!");
            return;
        }

        ChangeState(victory ? GameState.Victory : GameState.GameOver);

        if (victory)
            EventBus.Fire(new CarReachedEndEvent());
        else
            EventBus.Fire(new GameOverEvent());
        
        LogFinalStats(victory);
    }
    
    public void RestartGame()
    {
        if (currentState == GameState.Playing)
        {
            Debug.LogWarning("Не можна перезапустити гру поки вона активна!");
            return;
        }
        
        ResetGameState();
        
        // Повертаємося в меню
        ChangeState(GameState.Menu);
    }
    
    private void ResetGameState()
    {
        inputController.ResetForNewGame();
    }
    private void LogFinalStats(bool isWin)
    {
        Debug.Log("=== FINAL STATS ===");
        Debug.Log($"Результат: {(isWin ? "ПЕРЕМОГА" : "ПОРАЗКА")}");
        Debug.Log($"Фінальне HP: {hpManager.CurrentHP}/{hpManager.MaxHP}");
        Debug.Log($"Фінальна позиція: {carController.Position}");
        Debug.Log($"Швидкість: {carController.CurrentSpeed}");
        Debug.Log($"Фінальний стан: {currentState}");
        Debug.Log("==================");
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}