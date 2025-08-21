using UnityEngine;
using Zenject;

public class StartGameFromCheckpointTileService : MonoBehaviour
{
    [SerializeField] private GatesController gatesController;

    public GatesController GatesController => gatesController;
    private IEventBus _eventBus;
    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;
        Debug.LogWarning("StartGameFromCheckpointTileService Injected");
        Init();
    }

    private void Init()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        _eventBus.Subscribe<ReadyGameEvent>(OnGameReady);
        _eventBus.Subscribe<RestarGameEvent>(OnGameRestart);
    }

    private void UnSubscribe()
    {
        _eventBus?.Unsubscribe<ReadyGameEvent>(OnGameReady);
        _eventBus?.Unsubscribe<RestarGameEvent>(OnGameRestart);
    }

    private void OnGameReady(ReadyGameEvent readyGameEvent)
    {
        gatesController.OpenGates();
    }

    private void OnGameRestart(RestarGameEvent restartGameEvent)
    {
        gatesController.ResetGates();
    }
    
    private void OnDestroy()
    {
        UnSubscribe();
    }
}