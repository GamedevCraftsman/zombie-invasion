using UnityEngine;
using Zenject;

public class CheckpointTileService : ICheckpointTileService
{
    private readonly GameSettings _gameSettings;
    private readonly DiContainer _container;

    private GameObject _checkpointTile;
    private StartGameFromCheckpointTileService _startGameFromCheckpointTileService;

    public GameObject CheckPointTile => _checkpointTile;
    [Inject]
    public CheckpointTileService(GameSettings gameSettings, DiContainer container)
    {
        _gameSettings = gameSettings;
        _container = container;
    }

    public void SpawnTile(Transform container)
    {
        _checkpointTile = _container.InstantiatePrefab(_gameSettings.CheckpointTilePrefab, container);
        _startGameFromCheckpointTileService = _checkpointTile.GetComponent<StartGameFromCheckpointTileService>();
    }

    public void MoveTile(Vector3 position)
    {
        ResetTile();
        
        _checkpointTile.transform.position = position;
    }

    private void ResetTile()
    {
        _startGameFromCheckpointTileService.GatesController.ResetGates();
    }
}