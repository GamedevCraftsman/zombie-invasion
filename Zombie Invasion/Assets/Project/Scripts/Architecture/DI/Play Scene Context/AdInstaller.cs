using UnityEngine;
using Zenject;

public class AdInstaller : MonoInstaller
{
    [SerializeField] private AdService adService;
    
    public override void InstallBindings()
    {
        Container.Bind<AdService>().FromInstance(adService).AsSingle();
    }
}