using System;

public interface IRewardedAdService
{
    event Action OnGiveReward;
    public event Action OnFailedLoadAd;
    void LoadRewardedAd();
}