using UnityEngine;
using Zenject;

public class FirebaseInstaller : MonoInstaller
{
    [SerializeField] private FirebaseSettings firebaseSettings;
    
    public override void InstallBindings()
    {
        Container.Bind<IFirebaseSystemService>().To<FirebaseSystemService>().AsSingle();
        
        //Settings
        Container.Bind<FirebaseSettings>().FromInstance(firebaseSettings).AsSingle();
    }
}