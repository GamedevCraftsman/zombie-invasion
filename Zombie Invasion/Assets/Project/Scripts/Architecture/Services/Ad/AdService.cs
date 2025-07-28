using System;
using Unity.Services.LevelPlay;
using UnityEngine;

public class AdService : MonoBehaviour
{
#if UNITY_ANDROID
    private const string AppKey = "230b9dcc5";
#elif Unity_IPHONE
    private const string AppKey = "";
#else
    private const string AppKey = "unexpected_platform";
#endif

    private const string BannerID = "q13pj3ztxjvtl4bv";
    private const string RewardedID = "04mjv6kgu41pixtx";
    
    private void Start()
    {
        //Set test suite meta data.
        //LevelPlay.SetMetaData("is_test_suite", "enable"); 
        
        LevelPlay.ValidateIntegration();
        LevelPlay.Init(AppKey);

    }

    public void ShowRewardedAd(Action onRewarded)
    {
        var rewardAd = new RewardedAdService(RewardedID);
        rewardAd.OnGiveReward += onRewarded;
        
        rewardAd.LoadRewardedAd();
    }

    private void OnEnable()
    {
        LevelPlay.OnInitSuccess += OnSDKInitSuccess;
        LevelPlay.OnInitFailed += OnSDKInitFailed;
    }

    private void OnDestroy()
    {
        LevelPlay.OnInitSuccess -= OnSDKInitSuccess;
        LevelPlay.OnInitFailed += OnSDKInitFailed;
    }

    private void OnSDKInitSuccess(LevelPlayConfiguration config)
    {
        Debug.LogWarning($"SDK initialized. IsAdQualityEnabled: {config.IsAdQualityEnabled}");
        
        //Open test suite.
        //LevelPlay.LaunchTestSuite();
        
        InitBanner();
    }

    private void OnSDKInitFailed(LevelPlayInitError error)
    {
        Debug.LogWarning($"SDK initialize failed:\n {error.ErrorCode}: {error.ErrorMessage}");
    }

    private void InitBanner()
    {
        var bannerAdService = new BannerAdService(BannerID);
        bannerAdService.LoadBannerAd();
    }
}