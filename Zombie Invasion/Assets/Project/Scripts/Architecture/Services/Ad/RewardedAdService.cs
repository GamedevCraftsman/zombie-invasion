using System;
using UnityEngine;
using Unity.Services.LevelPlay;
using Zenject;

public class RewardedAdService : IRewardedAdService, IDisposable
{
    private readonly AdSettings _adSettings;
    private LevelPlayRewardedAd _rewardedAd;

    public event Action OnGiveReward;
    
    [Inject]
    public RewardedAdService(AdSettings adSettings)
    {
        _adSettings = adSettings;
        
        CreateRewardedAd();
    }
    
    private void CreateRewardedAd()
    {
        //Create RewardedAd instance
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
        }
    }

    private void DestroyRewardedAd()
    {
        _rewardedAd.DestroyAd();
    }
    
    //Implement RewardedAd events
    void RewardedOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.LogWarning("Rewarded ad loaded");
        ShowRewardedAd();
    }

    void RewardedOnAdLoadFailedEvent(LevelPlayAdError ironSourceError)
    {
        Debug.LogWarning($"Rewarded ad load failed. {ironSourceError.ErrorCode}: {ironSourceError.ErrorMessage}");
    }

    void RewardedOnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward adReward)
    {
        //Give reward
        OnGiveReward?.Invoke();
    }

    public void Dispose()
    {
        UnsubscribeRewardedMethods();
        DestroyRewardedAd();
        
        Debug.LogWarning("Rewarded ad destroyed");
    }
}