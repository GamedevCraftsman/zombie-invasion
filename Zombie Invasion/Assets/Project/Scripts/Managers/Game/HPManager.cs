using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class HPManager : BaseManager
{
    // Dependencies
    [Inject] private CarSettings carSettings;
    
    // State
    private int currentHP;
    private int maxHP;
    
    // Properties
    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;
    public float HPPercentage => maxHP > 0 ? (float)currentHP / maxHP : 0f;
    public bool IsAlive => currentHP > 0;
    
    protected override Task Initialize()
    {
        maxHP = carSettings.MaxHP;
        ResetHP();
        
        SubscribeToEvents();
        
        Debug.Log($"HPManager ініціалізовано. MaxHP: {maxHP}");
        return Task.CompletedTask;
    }
    
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<StartGameEvent>(OnGameStarted);
        EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }
    
    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<StartGameEvent>(OnGameStarted);
        EventBus?.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }
    
    private void OnGameStarted(StartGameEvent startEvent)
    {
        ResetHP();
        Debug.Log($"HP скинуто до {currentHP}/{maxHP}");
    }
    
    private void OnPlayerDamaged(PlayerDamagedEvent damageEvent)
    {
        TakeDamage(damageEvent.DamageAmount);
    }
    
    private void ResetHP()
    {
        currentHP = maxHP;
        FireHPChangedEvent();
    }
    
    private void TakeDamage(int damageAmount)
    {
        if (damageAmount <= 0)
        {
            Debug.LogWarning("Спроба завдати від'ємного або нульового урону!");
            return;
        }
        
        int previousHP = currentHP;
        currentHP = Mathf.Max(0, currentHP - damageAmount);
        
        Debug.Log($"Отримано {damageAmount} урону. HP: {previousHP} → {currentHP}");
        
        // Сповіщаємо UI про зміну HP
        FireHPChangedEvent();
        
        // Перевіряємо, чи гравець ще живий
        if (currentHP <= 0 && previousHP > 0)
        {
            Debug.Log("HP дійшло до 0! Запускаємо Game Over...");
            EventBus.Fire(new GameOverEvent(false));
        }
    }
    
    private void FireHPChangedEvent()
    {
        EventBus.Fire(new HPChangedEvent(currentHP, maxHP));
    }
    
    // Public methods для зовнішнього використання
    public void Heal(int healAmount)
    {
        if (healAmount <= 0) return;
        
        int previousHP = currentHP;
        currentHP = Mathf.Min(maxHP, currentHP + healAmount);
        
        Debug.Log($"Відновлено {healAmount} HP. HP: {previousHP} → {currentHP}");
        
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
        
        maxHP = newMaxHP;
        currentHP = Mathf.Min(currentHP, maxHP);
        
        Debug.Log($"MaxHP змінено на {maxHP}. Поточне HP: {currentHP}");
        
        // Сповіщаємо UI про зміну HP
        FireHPChangedEvent();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}