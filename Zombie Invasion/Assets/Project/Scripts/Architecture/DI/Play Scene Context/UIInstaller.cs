using UnityEngine;
using Zenject;

public class UIInstaller : MonoInstaller
{
    [Header("Canvas groups")]
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;
    
    [Header("Settings")]
    [SerializeField] private GameplayUISettings gameplayUISettings;
    [SerializeField] private UISettings uiSettings;

    public override void InstallBindings()
    {
        //Settings
        Container.BindInstance(uiSettings).AsSingle();
        Container.BindInstance(gameplayUISettings).AsSingle();

        //Managers
        Container.Bind<IUIManager>().To<UIManager>().AsSingle();
        
        //Services
        Container.BindInterfacesTo<MainScreenUIChanger>().AsSingle().WithArguments(mainMenuCanvasGroup);
        Container.BindInterfacesTo<MainScreenEventSubscriber>().AsSingle().NonLazy();
    }
}