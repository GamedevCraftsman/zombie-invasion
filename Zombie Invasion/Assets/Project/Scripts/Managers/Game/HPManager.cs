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
            _maxHp = _carSettings.MaxHp;
            ResetHp();

            SubscribeToEvents();
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
        ResetHp();
    }

    private void OnPlayerDamaged(PlayerDamagedEvent damageEvent)
    {
        TakeDamage(damageEvent.DamageAmount);
    }

    private void OnGameRestart(RestarGameEvent restartEvent)
    {
        ResetHp();
    }

    private void OnGameContinue(ContinueGameEvent continueEvent)
    {
        ResetHp();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    #endregion

    private void ResetHp()
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

        ChangeHp(damageAmount);
    }

    private void ChangeHp(int damageAmount)
    {
        int previousHp = _currentHp;
        _currentHp = Mathf.Max(0, _currentHp - damageAmount);

        FireHpChangedEvent();
        IsLoseHp(previousHp);
    }

    private void FireHpChangedEvent()
    {
        EventBus.Fire(new HPChangedEvent(_currentHp, _maxHp));
    }

    private void IsLoseHp(int previousHp)
    {
        if (_currentHp <= 0 && previousHp > 0)
        {
            _gameManager.EndGame(false);
        }
    }
}