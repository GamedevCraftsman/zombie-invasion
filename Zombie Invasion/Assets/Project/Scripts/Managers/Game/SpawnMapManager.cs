using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class SpawnMapManager : BaseManager
{
    [SerializeField] private GameObject groundTilesContainer;

    private readonly List<GameObject> _groundTiles = new();
    private Vector3 _startPosition;
    private GameSettings _gameSettings;
    private IProgressIncreaseService _progressIncreaseService;
    private ICheckpointTileService _checkpointTileService;

    // Public access to ground tiles for enemy spawn system
    public List<GameObject> GroundTiles => _groundTiles;

    [Inject]
    private void Construct(GameSettings gameSettings, IProgressIncreaseService progressIncreaseService,
        ICheckpointTileService checkpointTileService)
    {
        _gameSettings = gameSettings;
        _progressIncreaseService = progressIncreaseService;
        _checkpointTileService = checkpointTileService;

        _startPosition = new Vector3(0, 0, 0);
    }

    protected override Task Initialize()
    {
        try
        {
            SubscribeToEvents();
            _checkpointTileService.SpawnTile(groundTilesContainer.transform);
            _checkpointTileService.MoveTile(_startPosition);
            ManageGroundTiles(false);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return Task.CompletedTask;
    }

    #region Events

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<ContinueGameEvent>(OnContinueGame);
        EventBus.Subscribe<RestarGameEvent>(OnGameRestart);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<ContinueGameEvent>(OnContinueGame);
        EventBus?.Unsubscribe<RestarGameEvent>(OnGameRestart);
    }

    private void OnContinueGame(ContinueGameEvent continueGameEvent)
    {
        ManageGroundTiles(false);
    }

    private void OnGameRestart(RestarGameEvent restartGameEvent)
    {
        ManageGroundTiles(true);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    #endregion

    private void ManageGroundTiles(bool isRestart)
    {
        //Remove nulls
        _groundTiles.RemoveAll(tile => tile == null);

        SpawnMissingTiles(_gameSettings.MapLength + _progressIncreaseService.MapIncrease);

        RepositionAllTiles(isRestart);
    }

    private void SpawnMissingTiles(int requiredCount)
    {
        int currentCount = _groundTiles.Count;

        if (currentCount < requiredCount)
        {
            int tilesToSpawn = requiredCount - currentCount;

            for (int i = 0; i < tilesToSpawn; i++)
            {
                GameObject newTile = Instantiate(_gameSettings.MapTilePrefab, groundTilesContainer.transform);
                _groundTiles.Add(newTile);
            }
        }
    }

    private void RepositionAllTiles(bool isRestart)
    {
        Vector3 repositionStartPosition = GetRepositionStartPositionAdvanced(isRestart);
        
        // Move to new positions
        //_checkpointTileService.MoveTile(repositionStartPosition);
        //repositionStartPosition +=  Vector3.forward * _gameSettings.DistanceBetweenTiles;
        
        for (int i = 0; i < _groundTiles.Count; i++)
        {
            if (_groundTiles[i] != null)
            {
                Vector3 newTilePosition =
                    repositionStartPosition + Vector3.forward * ((i + 1) * _gameSettings.DistanceBetweenTiles);
                _groundTiles[i].transform.localPosition = newTilePosition;
            }
        }
    }

    private Vector3 GetRepositionStartPositionAdvanced(bool isRestart)
    {
        float maxZ = _checkpointTileService.CheckPointTile.transform.position.z;
        bool foundAnyTile = false;

        foreach (GameObject tile in _groundTiles)
        {
            if (tile != null)
            {
                foundAnyTile = true;
                if (tile.transform.position.z > maxZ)
                {
                    maxZ = tile.transform.position.z;
                }
            }
        }

        if (foundAnyTile && !isRestart)
        {        
            return new Vector3(_startPosition.x, _startPosition.y, maxZ); //return last tile position
        }

        _checkpointTileService.MoveTile(_startPosition);
        return _startPosition;
    }
}