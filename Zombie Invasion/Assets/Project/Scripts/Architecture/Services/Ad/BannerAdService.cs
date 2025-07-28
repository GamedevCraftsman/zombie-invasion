using UnityEngine;
using Unity.Services.LevelPlay;

public class BannerAdService
{
    private LevelPlayBannerAd _bannerAd;

    public BannerAdService(string bannerAdPath)
    {
        CreateBannerAd(bannerAdPath);
    }

    void CreateBannerAd(string bannerAdPath)
    {
       CreateInstance(bannerAdPath);

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

    #region Events

    private void SubscribeEvents()
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
    
    
}