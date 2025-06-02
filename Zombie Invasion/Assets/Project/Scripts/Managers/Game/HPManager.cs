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

        FireHPChangedEvent();

        if (_currentHP <= 0 && previousHP > 0)
        {
            _gameManager.EndGame(false);
        }
    }

    private void FireHPChangedEvent()
    {
        EventBus.Fire(new HPChangedEvent(_currentHP, _maxHP));
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}