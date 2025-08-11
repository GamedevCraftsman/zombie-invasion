using UnityEngine;
using Zenject;

public class ProgressSystemInstaller : MonoInstaller
{
    [SerializeField] private ProgressSettings progressSettings;
    
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<ProgressIncreaseService>().AsSingle();
        
        //Settings
        Container.Bind<ProgressSettings>().FromInstance(progressSettings).AsSingle();
    }
}