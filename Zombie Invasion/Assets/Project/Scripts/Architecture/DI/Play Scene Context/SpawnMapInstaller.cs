using UnityEngine;
using Zenject;

public class SpawnMapInstaller : MonoInstaller
{
    [Header("Settings")]
    [SerializeField] private GameSettings gameSettings;
    
    [Header("Managers")]
    [SerializeField] private SpawnMapManager spawnMapManager;
    
    public override void InstallBindings()
    {
        //Settings
        Container.Bind<GameSettings>().FromInstance(gameSettings).AsSingle();
        
        //Managers
        Container.Bind<SpawnMapManager>().FromInstance(spawnMapManager).AsSingle();
    }
}