using System;

public interface IAdService
{
    void ShowRewardedAd(Action onGiveReward,  Action onFailedLoadAd);
}