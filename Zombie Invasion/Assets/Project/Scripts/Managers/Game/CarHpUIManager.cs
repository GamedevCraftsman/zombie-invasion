using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CarHpUIManager : BaseManager
{
    private ICarHPUIController _carHpUIController;

    [Inject]
    public void Construct(ICarHPUIController carHpUIController)
    {
        _carHpUIController = carHpUIController;
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
        EventBus.Subscribe<HPChangedEvent>(OnHPChanged);
        EventBus.Subscribe<StartGameEvent>(OnGameStarted);
        EventBus.Subscribe<GameOverEvent>(OnGameEnd);
        EventBus.Subscribe<CarReachedEndEvent>(OnGameEnd);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus.Unsubscribe<GameOverEvent>(OnGameEnd);
        EventBus.Unsubscribe<CarReachedEndEvent>(OnGameEnd);
        EventBus?.Unsubscribe<HPChangedEvent>(OnHPChanged);
        EventBus?.Unsubscribe<StartGameEvent>(OnGameStarted);
    }

    private void OnGameStarted(StartGameEvent startEvent)
    {
        _carHpUIController.ResetUI();
        _carHpUIController.ShowHpBar();
    }

    private void OnGameEnd(GameOverEvent gameOverEvent)
    {
        _carHpUIController.HideHpBar();
    }

    private void OnGameEnd(CarReachedEndEvent carReachedEndEvent)
    {
        _carHpUIController.HideHpBar();
    }

    private void OnHPChanged(HPChangedEvent hpEvent)
    {
        _carHpUIController.UpdateHp(hpEvent.HpPercentage, hpEvent.CurrentHp, hpEvent.MaxHp);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}