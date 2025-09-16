using TMPro;
using UnityEngine;
using Zenject;

public class ProgressSystemInstaller : MonoInstaller
{
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private ProgressSettings progressSettings;
    
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<ProgressIncreaseService>().AsSingle();
        Container.BindInterfacesTo<ProgressUIUpdateService>().AsSingle().WithArguments(progressText);
        Container.Bind<ProgressSettings>().FromInstance(progressSettings);
    }
}