using UnityEngine;
using Zenject;

public class AdInstaller : MonoInstaller
{
    [Header("Settings")]
    [SerializeField] private AdSettings adSettings;
    
    public override void InstallBindings()
    {
        //Services
        Container.BindInterfacesTo<AdService>().AsSingle();
        Container.BindInterfacesTo<BannerAdService>().AsSingle();
        Container.BindInterfacesTo<RewardedAdService>().AsSingle();
        Container.BindInterfacesTo<InterstitialAdService>().AsSingle();
    
        //Settings
        Container.Bind<AdSettings>().FromInstance(adSettings).AsSingle();
    
    }
}