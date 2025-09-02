using UnityEngine;
using Zenject;

public class FirebaseInstaller : MonoInstaller
{
    [Header("Firebase")]
    [SerializeField] private FirebaseSettings firebaseSettings;
    
    [Header("Additional")]
    [SerializeField] private ProgressSettings progressSettings;
    
    public override void InstallBindings()
    {
        Container.Bind<IFirebaseSystemService>().To<FirebaseSystemService>().AsSingle().NonLazy();
        Container.Bind<DataManageService>().AsSingle().NonLazy();
        
        //Settings
        Container.Bind<FirebaseSettings>().FromInstance(firebaseSettings).AsSingle().NonLazy();
        
        //Additional Settings
        Container.Bind<ProgressSettings>().FromInstance(progressSettings).AsSingle().NonLazy();
    }
}