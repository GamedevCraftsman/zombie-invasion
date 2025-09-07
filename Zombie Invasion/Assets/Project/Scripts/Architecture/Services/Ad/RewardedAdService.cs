using System;
using UnityEngine;
using Unity.Services.LevelPlay;
using Zenject;

public class RewardedAdService : IRewardedAdService, IDisposable
{
    private readonly AdSettings _adSettings;
    private LevelPlayRewardedAd _rewardedAd;

    public event Action OnGiveReward;
    public event Action OnFailedLoadAd;

    [Inject]
    public RewardedAdService(AdSettings adSettings)
    {
        _adSettings = adSettings;

        CreateRewardedAd();
    }

    private void CreateRewardedAd()
    {
        _rewardedAd = new LevelPlayRewardedAd(_adSettings.RewardedAdUnitId);

        SubscribeRewardedMethods();
    }

    private void SubscribeRewardedMethods()
    {
        _rewardedAd.OnAdLoaded += RewardedOnAdLoadedEvent;
        _rewardedAd.OnAdLoadFailed += RewardedOnAdLoadFailedEvent;
        _rewardedAd.OnAdRewarded += RewardedOnAdRewarded;
    }

    private void UnsubscribeRewardedMethods()
    {
        _rewardedAd.OnAdLoaded -= RewardedOnAdLoadedEvent;
        _rewardedAd.OnAdLoadFailed -= RewardedOnAdLoadFailedEvent;
        _rewardedAd.OnAdRewarded -= RewardedOnAdRewarded;
    }

    public void LoadRewardedAd()
    {
        _rewardedAd.LoadAd();
    }

    private void ShowRewardedAd()
    {
        if (_rewardedAd.IsAdReady())
        {
            _rewardedAd.ShowAd("Game_Screen");
            ;
        }
    }

    private void DestroyRewardedAd()
    {
        _rewardedAd.DestroyAd();
    }

    private void RewardedOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.LogWarning("Rewarded ad loaded");
        ShowRewardedAd();
    }

    private void RewardedOnAdLoadFailedEvent(LevelPlayAdError ironSourceError)
    {
        Debug.LogWarning($"Rewarded ad load failed. {ironSourceError.ErrorCode}: {ironSourceError.ErrorMessage}");
        OnFailedLoadAd?.Invoke();
        CleanActions();
    }

    private void RewardedOnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward adReward)
    {
        //Give reward
        OnGiveReward?.Invoke();
        CleanActions();
    }

    private void CleanActions()
    {
        OnFailedLoadAd = null;
        OnGiveReward = null;
    }

    public void Dispose()
    {
        UnsubscribeRewardedMethods();
        DestroyRewardedAd();

        Debug.LogWarning("Rewarded ad destroyed");
    }
}