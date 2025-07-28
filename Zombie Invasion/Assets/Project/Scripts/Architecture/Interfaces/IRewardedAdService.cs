using System;

public interface IRewardedAdService
{
    event Action OnGiveReward;
    void LoadRewardedAd();
}