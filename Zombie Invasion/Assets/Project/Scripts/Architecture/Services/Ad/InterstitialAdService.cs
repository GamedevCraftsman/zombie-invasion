using System;
using Unity.Services.LevelPlay;
using Unity.VisualScripting;
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

    void CreateInterstitialAd()
    {
        //Create InterstitialAd instance
        _interstitialAd = new LevelPlayInterstitialAd(_adSettings.InterstitialAdUnitId);

        //Subscribe InterstitialAd events
        _interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        _interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        _interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        _interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        _interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        _interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        _interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
    }

    public void LoadInterstitialAd()
    {
        //Load or reload InterstitialAd 	
        _interstitialAd.LoadAd();
    }

    void ShowInterstitialAd()
    {
        //Show InterstitialAd, check if the ad is ready before showing
        if (_interstitialAd.IsAdReady())
        {
            _interstitialAd.ShowAd();
        }
    }

    void DestroyInterstitialAd()
    {
        //Destroy InterstitialAd 
        _interstitialAd.DestroyAd();
    }

    //Implement InterstitialAd events
    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
    }

    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError ironSourceError)
    {
    }

    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
    }

    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
    }

    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError adInfoError)
    {
    }

    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
    }

    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
    {
    }

    public void Dispose()
    {
        DestroyInterstitialAd();
        
        Debug.LogWarning("Interstitial ad disposed");
    }
}