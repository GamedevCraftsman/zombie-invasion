using UnityEngine;
using Zenject;

public class UIInstaller : MonoInstaller
{
    [SerializeField] private GameplayUISettings gameplayUISettings;
    [SerializeField] private UISettings uiSettings;

    public override void InstallBindings()
    {
        //Settings
        Container.BindInstance(uiSettings).AsSingle();
        Container.BindInstance(gameplayUISettings).AsSingle();

        //Managers
        Container.Bind<IUIManager>().To<UIManager>().AsSingle();
    }
}