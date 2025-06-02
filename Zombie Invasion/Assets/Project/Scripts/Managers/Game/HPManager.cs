using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class HPManager : BaseManager
{
    // Dependencies
    [Inject] private CarSettings _carSettings;
    [Inject] private IGameManager _gameManager;
    
    // State
    private int _currentHP;
    private int _maxHP;
    
    // Properties
    public int CurrentHP => _currentHP;
    public int MaxHP => _maxHP;
    public float HPPercentage => _maxHP > 0 ? (float)_currentHP / _maxHP : 0f;
    public bool IsAlive => _currentHP > 0;
    
    protected override Task Initialize()
    {
        _maxHP = _carSettings.MaxHP;
        ResetHP();
        
        SubscribeToEvents();
        
        Debug.Log($"HPManager ініціалізовано. MaxHP: {_maxHP}");
        return Task.CompletedTask;
    }
    
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<StartGameEvent>(OnGameStarted);
        EventBus.Subscribe<RestarGameEvent>(OnGameRestart);
        EventBus.Subscribe<ContinueGameEvent>(OnGameContinue);
        EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }
    
    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<StartGameEvent>(OnGameStarted);
        EventBus?.Unsubscribe<RestarGameEvent>(OnGameRestart);
        EventBus?.Unsubscribe<ContinueGameEvent>(OnGameContinue);
        EventBus?.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }
    
    private void OnGameStarted(StartGameEvent startEvent)
    {
        ResetHP();
        Debug.Log($"HP скинуто до {_currentHP}/{_maxHP}");
    }
    
    private void OnPlayerDamaged(PlayerDamagedEvent damageEvent)
    {
        TakeDamage(damageEvent.DamageAmount);
    }

    private void OnGameRestart(RestarGameEvent restartEvent)
    {
        ResetHP();
    }

    private void OnGameContinue(ContinueGameEvent continueEvent)
    {
        ResetHP();
    }
    
    private void ResetHP()
    {
        _currentHP = _maxHP;
        FireHPChangedEvent();
    }
    
    private void TakeDamage(int damageAmount)
    {
        if (damageAmount <= 0)
        {
            Debug.LogWarning("Спроба завдати від'ємного або нульового урону!");
            return;
        }
        
        int previousHP = _currentHP;
        _currentHP = Mathf.Max(0, _currentHP - damageAmount);
        
        Debug.Log($"Отримано {damageAmount} урону. HP: {previousHP} → {_currentHP}");
        
        // Сповіщаємо UI про зміну HP
        FireHPChangedEvent();
        
        // Перевіряємо, чи гравець ще живий
        if (_currentHP <= 0 && previousHP > 0)
        {
            Debug.Log("HP дійшло до 0! Запускаємо Game Over...");
            //EventBus.Fire(new GameOverEvent(false));
            _gameManager.EndGame(false);
        }
    }
    
    private void FireHPChangedEvent()
    {
        EventBus.Fire(new HPChangedEvent(_currentHP, _maxHP));
    }
    
    // Public methods для зовнішнього використання
    public void Heal(int healAmount)
    {
        if (healAmount <= 0) return;
        
        int previousHP = _currentHP;
        _currentHP = Mathf.Min(_maxHP, _currentHP + healAmount);
        
        Debug.Log($"Відновлено {healAmount} HP. HP: {previousHP} → {_currentHP}");
        
        // Сповіщаємо UI про зміну HP
        FireHPChangedEvent();
    }
    
    public void SetMaxHP(int newMaxHP)
    {
        if (newMaxHP <= 0)
        {
            Debug.LogError("MaxHP не може бути <= 0!");
            return;
        }
        
        _maxHP = newMaxHP;
        _currentHP = Mathf.Min(_currentHP, _maxHP);
        
        Debug.Log($"MaxHP змінено на {_maxHP}. Поточне HP: {_currentHP}");
        
        // Сповіщаємо UI про зміну HP
        FireHPChangedEvent();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}