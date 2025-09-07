using UnityEngine;
using Zenject;

public class CarInstaller : MonoInstaller
{
    [Header("Car Settings")]
    [SerializeField] private CarSettings carSettings;
    
    [Header("Managers")]
    [SerializeField] private HPManager hpManager;
    
    [Header("Controllers")]
    [SerializeField] private CarHPUIController carHpUIController;
    [SerializeField] private CarController carController;
    
    public override void InstallBindings()
    {
        //Managers
        Container.Bind<HPManager>().FromInstance(hpManager).AsSingle();
        
        //Controllers
        Container.Bind<ICarController>().FromInstance(carController).AsSingle();
        Container.Bind<ICarHPUIController>().FromInstance(carHpUIController).AsSingle();
        
        //Settings
        Container.BindInstance(carSettings).AsSingle();

    }
}
