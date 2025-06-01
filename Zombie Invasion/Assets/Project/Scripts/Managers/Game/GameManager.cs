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
    [Inject] private HPUIController hpUIController;
    
    // State
    private GameState currentState = GameState.Menu;
   // private bool isInitialized = false;
    
    // Properties
    public GameState CurrentState => currentState;
    //public bool IsInitialized => isInitialized;
    
    protected override Task Initialize()
    {
        try
        {
            SubscribeToEvents();
            ChangeState(GameState.Menu);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        
        
        // Debug.Log("GameManager ініціалізація почалася...");
        // return InitializeAllSystems();
        
        return Task.CompletedTask;
    }
    
    /*private async Task InitializeAllSystems()
    {
        try
        {
            /#1#/ Ініціалізуємо всі системи по черзі
            Debug.Log("Ініціалізація CarController...");
            await carController.InitializeAsync();
            
            Debug.Log("Ініціалізація HPManager...");
            await hpManager.InitializeAsync();
            
            Debug.Log("Ініціалізація HPUIController...");
            await hpUIController.InitializeAsync();
            
            Debug.Log("Ініціалізація InputController...");
            await inputController.InitializeAsync();
            
            isInitialized = true;
            Debug.Log("✅ GameManager: Всі системи ініціалізовано!");#1#
            
            // Встановлюємо початковий стан
            ChangeState(GameState.Menu);
            
            // Логування поточного стану
            //LogSystemStatus();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Помилка при ініціалізації GameManager: {ex.Message}");
            Debug.LogException(ex);
        }
    }*/
    
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<StartGameEvent>(OnStartGameEvent);
        //EventBus.Subscribe<CarReachedEndEvent>(OnCarReachedEnd);
        EventBus.Subscribe<GameOverEvent>(OnGameOverEvent);
        EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }
    
    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<StartGameEvent>(OnStartGameEvent);
        EventBus?.Unsubscribe<CarReachedEndEvent>(OnCarReachedEnd);
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
        Debug.Log("🏁 GameManager: Машина досягла кінця!");
        
        if (hpManager.IsAlive)
        {
            Debug.Log("🎉 Гравець жив до кінця - ПЕРЕМОГА!");
            EndGame(true);
        }
        // else
        // {
        //     Debug.Log("💀 Гравець мертвий - це не повинно статися!");
        //     EndGame(false);
        // }
    }
    
    private void OnGameOverEvent(GameOverEvent gameOverEvent)
    {
        // Цей event тепер використовується внутрішньо
        ChangeState(GameState.GameOver);
        
        
        // Логіка перенесена в EndGame()
    }
    
    private void OnPlayerDamaged(PlayerDamagedEvent damageEvent)
    {
        Debug.Log($"⚡ GameManager: Гравець отримав {damageEvent.DamageAmount} урону");
        
        // Перевіряємо, чи не помер гравець
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
        
        Debug.Log($"🔄 Зміна стану гри: {previousState} → {newState}");
        
        // Сповіщаємо про зміну стану
        EventBus.Fire(new GameStateChangedEvent(previousState, newState));
        
        // Виконуємо дії при вході в новий стан
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
        // Тут можна активувати UI меню, зупинити музику гри тощо
    }
    
    private void OnPlayingEntered()
    {
        Debug.Log("🎮 Увійшли в стан Playing");
        // Тут можна запустити ігрову музику, сховати меню тощо
    }
    
    private void OnGameOverEntered()
    {
        Debug.Log("💀 Увійшли в стан GameOver");
        // Тут можна показати екран поразки, зупинити музику тощо
    }
    
    private void OnVictoryEntered()
    {
        Debug.Log("🎉 Увійшли в стан Victory");
        // Тут можна показати екран перемоги, запустити святкову музику тощо
    }
    
    // Методи інтерфейсу IGameManager
    public void StartGame()
    {
        // if (!isInitialized)
        // {
        //     Debug.LogWarning("Спроба запустити гру до завершення ініціалізації!");
        //     return;
        // }
        
        if (currentState != GameState.Menu)
        {
            Debug.LogWarning($"Не можна запустити гру зі стану {currentState}!");
            return;
        }
        
        Debug.Log("🚀 Запуск гри...");
        
        // Скидаємо стан гри (якщо потрібно)
        ResetGameState();
        
        // Змінюємо стан на Playing
        ChangeState(GameState.Playing);
        
        // Відправляємо внутрішню подію для запуску систем
        // (StartGameEvent тепер обробляється внутрішньо)
    }
    
    public void EndGame(bool victory)
    {
        if (currentState != GameState.Playing)
        {
            Debug.LogWarning($"Не можна завершити гру зі стану {currentState}!");
            return;
        }
        
        string result = victory ? "ПЕРЕМОГА" : "ПОРАЗКА";
        Debug.Log($"🏆 Завершення гри - {result}");
        
        // Змінюємо стан
        ChangeState(victory ? GameState.Victory : GameState.GameOver);
        
        // Відправляємо подію для інших систем
        EventBus.Fire(new GameOverEvent(victory));
        
        // Логування фінальної статистики
        LogFinalStats(victory);
    }
    
    public void RestartGame()
    {
        if (currentState == GameState.Playing)
        {
            Debug.LogWarning("Не можна перезапустити гру поки вона активна!");
            return;
        }
        
        Debug.Log("🔄 Перезапуск гри...");
        
        // Скидаємо стан всіх систем
        ResetGameState();
        
        // Повертаємося в меню
        ChangeState(GameState.Menu);
        
        Debug.Log("Гра готова до нового запуску!");
    }
    
    private void ResetGameState()
    {
        // Скидаємо стан всіх систем
        //carController.ResetPosition();
        inputController.ResetForNewGame();
        
        // HP скинеться автоматично через StartGameEvent
    }
    
    //---------------------------------------------------------
    //Delete
    // Додаткові методи для зовнішнього контролю
    public void PauseGame()
    {
        if (currentState != GameState.Playing) return;
        
        Time.timeScale = 0f;
        Debug.Log("⏸️ Гра поставлена на паузу");
    }
    
    public void ResumeGame()
    {
        if (currentState != GameState.Playing) return;
        
        Time.timeScale = 1f;
        Debug.Log("Гра знята з паузи");
    }
    //---------------------------------------------------------
    // public bool CanStartGame()
    // {
    //     return isInitialized && currentState == GameState.Menu;
    // }
    
    public bool CanRestartGame()
    {
        return currentState == GameState.GameOver || currentState == GameState.Victory;
    }
    
    /*private void LogSystemStatus()
    {
        Debug.Log("=== SYSTEM STATUS ===");
        Debug.Log($"GameState: {currentState}");
        Debug.Log($"CarController готовий: {carController != null}");
        Debug.Log($"HPManager готовий: {hpManager != null}");
        Debug.Log($"HPUIController готовий: {hpUIController != null}");
        Debug.Log($"InputController готовий: {inputController != null}");
        Debug.Log($"Поточне HP: {hpManager?.CurrentHP}/{hpManager?.MaxHP}");
        Debug.Log($"Машина на позиції: {carController?.Position}");
        Debug.Log("====================");
    }*/
    
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
        Time.timeScale = 1f;
    }
}