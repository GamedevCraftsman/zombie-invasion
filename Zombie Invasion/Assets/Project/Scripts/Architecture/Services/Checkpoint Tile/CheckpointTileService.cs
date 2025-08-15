using UnityEngine;
using Zenject;

public class CheckpointTileService : ICheckpointTileService
{
    private readonly GameSettings _gameSettings;
    private readonly DiContainer _container;

    private GameObject _checkpointTile;

    public GameObject CheckPointTile => _checkpointTile;
    [Inject]
    public CheckpointTileService(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
    }

    public void SpawnTile(Transform container)
    {
        _checkpointTile =
            Object.Instantiate(_gameSettings.CheckpointTilePrefab,
                container); /*_container.InstantiatePrefab(_gameSettings.CheckpointTilePrefab, container);*/
    }

    public void MoveTile(Vector3 position)
    {
        ResetTile();
        
        _checkpointTile.transform.position = position;
    }

    private void ResetTile()
    {
        //Reset tile state.
    }
}