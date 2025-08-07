using UnityEngine;
using Zenject;

public class TransitionInstaller : MonoInstaller
{
    [SerializeField] private CanvasGroup transitionPanel;
    
    public override void InstallBindings()
    {
        Container.Bind<ITransitionService>().To<StandardTransition>().AsSingle().WithArguments(transitionPanel);
    }
}