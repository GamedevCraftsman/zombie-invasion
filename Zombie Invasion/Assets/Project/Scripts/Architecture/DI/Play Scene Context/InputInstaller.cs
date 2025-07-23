using Project.Scripts.Controllers;
using UnityEngine;
using Zenject;

public class InputInstaller : MonoInstaller
{
    [Header("Controllers")]
    [SerializeField] private InputController inputController;
    
    public override void InstallBindings()
    {
        Container.Bind<IInputController>().FromInstance(inputController).AsSingle();
    }
}