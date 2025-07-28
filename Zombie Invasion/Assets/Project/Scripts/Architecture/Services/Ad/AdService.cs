using System;
using Unity.Services.LevelPlay;
using UnityEngine;
using Zenject;

public class AdService : IDisposable, IAdService
{
    private readonly IBannerAdService _bannerAdService;
    private readonly IRewardedAdService _rewardedAdService;

    [Inject]
    private AdService(AdSettings adSettings, IBannerAdService bannerAdService, IRewardedAdService rewardedAdService)
    {
        _bannerAdService = bannerAdService;
        _rewardedAdService = rewardedAdService;
        
        InitializeAd(adSettings);
    }

    private void InitializeAd(AdSettings adSettings)
    {
        SubscribeEvents();
        LevelPlay.ValidateIntegration();
        LevelPlay.Init(adSettings.AppKey);

        Debug.LogWarning("AdService constructed");
    }

    public void ShowRewardedAd(Action onRewarded)
    {
        _rewardedAdService.OnGiveReward += onRewarded;
        _rewardedAdService.LoadRewardedAd();
    }

    private void SubscribeEvents()
    {
        LevelPlay.OnInitSuccess += OnSDKInitSuccess;
        LevelPlay.OnInitFailed += OnSDKInitFailed;
    }

    private void UnsubscribeEvents()
    {
        LevelPlay.OnInitSuccess -= OnSDKInitSuccess;
        LevelPlay.OnInitFailed -= OnSDKInitFailed;
    }

    private void OnSDKInitSuccess(LevelPlayConfiguration config)
    {
        Debug.LogWarning($"SDK initialized. IsAdQualityEnabled: {config.IsAdQualityEnabled}");
        InitBanner();
    }

    private void OnSDKInitFailed(LevelPlayInitError error)
    {
        Debug.LogWarning($"SDK initialize failed:\n {error.ErrorCode}: {error.ErrorMessage}");
    }

    private void InitBanner()
    {
        _bannerAdService.LoadBannerAd();
    }

    public void Dispose()
    {
        UnsubscribeEvents();
    }
}