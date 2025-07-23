using Zenject;
using UnityEngine;

public class CameraInstaller : MonoInstaller
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    
    public override void InstallBindings()
    {
        Container.Bind<Camera>().FromInstance(mainCamera).AsSingle();
    }
}