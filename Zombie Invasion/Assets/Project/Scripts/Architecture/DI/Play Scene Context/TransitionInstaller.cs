using UnityEngine;
using Zenject;

public class TransitionInstaller : MonoInstaller
{
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private TransitionPanelSettings transitionPanelSettings;
    
    public override void InstallBindings()
    {
        //Settings
        Container.Bind<TransitionPanelSettings>().FromInstance(transitionPanelSettings).AsSingle().NonLazy();
        
        //Services
        Container.Bind<ITransitionService>().To<StandardTransition>().AsSingle().WithArguments(transitionPanel).NonLazy();
    }
}