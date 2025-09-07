using UnityEngine;
using Zenject;

public class TurretInstaller : MonoInstaller
{
    [Header("Settings")]
    [SerializeField] private WeaponSettings weaponSettings;
    
    [Header("Controllers")]
    [SerializeField] private FireController fireController;
    [SerializeField] private TurretController turretController;
    [SerializeField] private AimStateService aimStateService;
    
    public override void InstallBindings()
    {
        //Settings
        Container.Bind<WeaponSettings>().FromInstance(weaponSettings).AsSingle();
        
        //Controllers
        Container.Bind<IFireController>().FromInstance(fireController).AsSingle();
        Container.Bind<ITurretController>().FromInstance(turretController).AsSingle();
        Container.Bind<IAimStateService>().FromInstance(aimStateService).AsSingle();
    }
}