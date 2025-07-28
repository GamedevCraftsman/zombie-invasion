using System;
using UnityEngine;
using Unity.Services.LevelPlay;

public class RewardedAdService
{
    private readonly string _adUnitId;
    private LevelPlayRewardedAd _rewardedAd;

    public event Action OnGiveReward;
    
    public RewardedAdService(string adUnitId)
    {
        _adUnitId = adUnitId;
        CreateRewardedAd();
    }
    
    void CreateRewardedAd()
    {
        //Create RewardedAd instance
        _rewardedAd = new LevelPlayRewardedAd(_adUnitId);
        
        SubscribeRewardedMethods();
    }

    private void SubscribeRewardedMethods()
    {
        _rewardedAd.OnAdLoaded += RewardedOnAdLoadedEvent;
        _rewardedAd.OnAdLoadFailed += RewardedOnAdLoadFailedEvent;
        _rewardedAd.OnAdRewarded += RewardedOnAdRewarded;
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
}