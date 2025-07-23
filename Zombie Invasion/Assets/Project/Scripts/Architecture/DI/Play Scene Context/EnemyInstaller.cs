using UnityEngine;
using Zenject;

public class EnemyInstaller : MonoInstaller
{
    [Header("Settings")]
    [SerializeField] private EnemySettings enemySettings;
    
    public override void InstallBindings()
    {
        //Settings
        Container.Bind<EnemySettings>().FromInstance(enemySettings).AsSingle();
        
        //Services
        Container.Bind<IEnemyAttack>().To<EnemyAttack>().AsTransient();
        Container.Bind<IEnemyHealthBarService>().To<EnemyHealthBarService>().AsSingle();

    }
}