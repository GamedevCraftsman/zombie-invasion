using UnityEngine;
using Zenject;

public class GameManagerInstaller : MonoInstaller
{
    [Header("Managers")]
    [SerializeField] private GameManager gameManager;

    public override void InstallBindings()
    {
        Container.Bind<IGameManager>().FromInstance(gameManager).AsSingle();
    }
}