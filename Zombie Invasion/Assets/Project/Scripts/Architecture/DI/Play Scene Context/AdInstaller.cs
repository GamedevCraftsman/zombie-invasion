using UnityEngine;
using Zenject;

public class AdInstaller : MonoInstaller
{
    [Header("Settings")]
    [SerializeField] private AdSettings adSettings;
    
    public override void InstallBindings()
    {
        //Services
        Container.BindInterfacesTo<AdService>().AsSingle().NonLazy();
        Container.BindInterfacesTo<BannerAdService>().AsSingle().NonLazy();
        Container.BindInterfacesTo<RewardedAdService>().AsSingle().NonLazy();
        Container.BindInterfacesTo<InterstitialAdService>().AsSingle().NonLazy();
    
        //Settings
        Container.Bind<AdSettings>().FromInstance(adSettings).AsSingle().NonLazy();
    
        //Additional
        Container.BindInterfacesTo<InterstitialAdCaller>().AsSingle().NonLazy();
    }
}