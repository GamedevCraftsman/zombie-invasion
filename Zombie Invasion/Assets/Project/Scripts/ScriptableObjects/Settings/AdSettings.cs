using UnityEngine;

[CreateAssetMenu(fileName = "AdSettings", menuName = "Ad/Ad Settings")]
public class AdSettings : ScriptableObject
{
    [Header("General Settings")] 
    [SerializeField] private string androidAppKey;
    [SerializeField] private string iphoneAppKey;

    [Header("Ad type settings")] 
    [SerializeField] private string bannerUnitId;

    [SerializeField] private string rewardedAdUnitId;
    [SerializeField] private string interstitialAdUnitId;

    #region Public values

    //App key
#if UNITY_ANDROID
    public string AppKey => androidAppKey;
#elif UNITY_IPHONE
    public string AppKey => iphoneAppKey;
#else
    public string AppKey => "unexpected_platform";
#endif

    //Ad type ID`s
    public string BannerUnitId => bannerUnitId;
    public string RewardedAdUnitId => rewardedAdUnitId;
    public string InterstitialAdUnitId => interstitialAdUnitId;

    #endregion
}