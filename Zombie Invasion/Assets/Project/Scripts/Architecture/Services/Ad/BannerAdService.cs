using System;
using UnityEngine;
using Unity.Services.LevelPlay;
using Zenject;

public class BannerAdService : IBannerAdService, IDisposable
{
    private readonly AdSettings _adSettings;
    private LevelPlayBannerAd _bannerAd;

    [Inject]
    public BannerAdService(AdSettings adSettings)
    {
        _adSettings = adSettings;
        
        CreateBannerAd();
    }

    private void CreateBannerAd()
    {
       CreateInstance(_adSettings.BannerUnitId);

       SubscribeEvents();
    }

    private void CreateInstance(string bannerAdPath)
    {
        _bannerAd = new LevelPlayBannerAd(
            adUnitId: bannerAdPath,
            placementName: "Game_Screen");
    }
    
    public void LoadBannerAd()
    {
        _bannerAd.LoadAd();
    }

    private void DestroyBannerAd()
    {
        _bannerAd.DestroyAd();
    }
    
    #region Events

    private void SubscribeEvents()
    {
        _bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        _bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
    }
    
    private void UnsubscribeEvents()
    {
        _bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        _bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
    }
    
    private void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.LogWarning($"Banner ad loaded!");
        
        _bannerAd.ShowAd();
    }

    private void BannerOnAdLoadFailedEvent(LevelPlayAdError ironSourceError)
    {
        Debug.LogWarning($"Banner ad load failed. {ironSourceError.ErrorCode}: {ironSourceError.ErrorMessage}");
    }

    #endregion


    public void Dispose()
    {
        UnsubscribeEvents();
        DestroyBannerAd();
        
        Debug.LogWarning("Banner ad destroyed!");
    }
}