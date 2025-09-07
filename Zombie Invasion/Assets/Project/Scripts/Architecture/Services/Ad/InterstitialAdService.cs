using System;
using Unity.Services.LevelPlay;
using UnityEngine;
using Zenject;

public class InterstitialAdService : IInterstitialAdService, IDisposable
{
    private readonly AdSettings _adSettings;
    private LevelPlayInterstitialAd _interstitialAd;

    [Inject]
    public InterstitialAdService(AdSettings adSettings)
    {
        _adSettings = adSettings;

        CreateInterstitialAd();
    }

    #region Events

    private void SubscribeEvents()
    {
        _interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        _interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
    }

    private void UnsubscribeEvents()
    {
        _interstitialAd.OnAdLoaded -= InterstitialOnAdLoadedEvent;
        _interstitialAd.OnAdLoadFailed -= InterstitialOnAdLoadFailedEvent;
    }

    private void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.LogWarning("Interstitial ad loaded");

        ShowInterstitialAd();
    }

    private void InterstitialOnAdLoadFailedEvent(LevelPlayAdError ironSourceError)
    {
        Debug.LogWarning($"Interstitial ad load failed. {ironSourceError.ErrorCode}: {ironSourceError.ErrorMessage}");
    }

    #endregion

    void CreateInterstitialAd()
    {
        _interstitialAd = new LevelPlayInterstitialAd(_adSettings.InterstitialAdUnitId);

        SubscribeEvents();
    }

    public void LoadInterstitialAd()
    {
        _interstitialAd.LoadAd();
        Debug.LogWarning("Interstitial ad loaded");
    }

    private void ShowInterstitialAd()
    {
        if (_interstitialAd.IsAdReady())
        {
            Debug.LogWarning("Interstitial ad is ready");
            _interstitialAd.ShowAd(placementName: "Game_Screen");
        }
    }

    private void DestroyInterstitialAd()
    {
        _interstitialAd.DestroyAd();
    }

    public void Dispose()
    {
        UnsubscribeEvents();
        DestroyInterstitialAd();

        Debug.LogWarning("Interstitial ad disposed");
    }
}