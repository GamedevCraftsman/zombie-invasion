using System;
using UnityEngine;
using Zenject;

public class InterstitialAdCaller : IDisposable
{
    private readonly IEventBus _eventBus;
    private readonly IInterstitialAdService _interstitialAdService;

    private int _countOfRounds;
    [Inject]
    public InterstitialAdCaller(IEventBus eventBus, IInterstitialAdService interstitialAdService)
    {
        _eventBus = eventBus;
        _interstitialAdService = interstitialAdService;
        
        Initialize();
    }

    private void Initialize()
    {
        SubscribeEvent();
    }

    #region Events

    private void SubscribeEvent()
    {
        _eventBus.Subscribe<CarReachedEndEvent>(OnCarReached);
    }

    private void UnsubscribeEvent()
    {
        _eventBus?.Unsubscribe<CarReachedEndEvent>(OnCarReached);
    }

    private void OnCarReached(CarReachedEndEvent carReachedEndEvent)
    {
        CountToShowAd();
    }

    #endregion

    private void CountToShowAd()
    {
        _countOfRounds++;

        if (_countOfRounds % 2 == 0)
        {
            _interstitialAdService.LoadInterstitialAd();
            _countOfRounds = 0;
        }
        
    }

    public void Dispose()
    {
        UnsubscribeEvent();
        
        Debug.LogWarning("Disposing InterstitialAdCaller");
    }
}