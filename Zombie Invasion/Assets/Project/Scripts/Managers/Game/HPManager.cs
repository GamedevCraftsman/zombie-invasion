using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class HPManager : BaseManager
{
    // Dependencies
    private CarSettings _carSettings;
    private IGameManager _gameManager;

    // State
    private int _currentHp;
    private int _maxHp;
    
    public bool IsAlive => _currentHp > 0;

    [Inject]
    public void Construct(CarSettings carSettings, IGameManager gameManager)
    {
        _carSettings = carSettings;
        _gameManager = gameManager;
    }
    
    protected override Task Initialize()
    {
        try
        {
            _maxHp = _carSettings.MaxHP;
            ResetHP();

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
        _currentHp = _maxHp;
        FireHpChangedEvent();
    }

    private void TakeDamage(int damageAmount)
    {
        if (damageAmount <= 0)
        {
            Debug.LogWarning("Try to take zero damage!");
            return;
        }

        int previousHP = _currentHp;
        _currentHp = Mathf.Max(0, _currentHp - damageAmount);

        FireHpChangedEvent();

        if (_currentHp <= 0 && previousHP > 0)
        {
            _gameManager.EndGame(false);
        }
    }

    private void FireHpChangedEvent()
    {
        EventBus.Fire(new HPChangedEvent(_currentHp, _maxHp));
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}