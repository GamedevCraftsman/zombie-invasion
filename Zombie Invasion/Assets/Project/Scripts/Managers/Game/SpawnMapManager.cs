using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class SpawnMapManager : BaseManager
{
    [SerializeField] private GameObject groundTilesContainer;
    
    private readonly List<GameObject> _groundTiles = new();
    private readonly Vector3 _startPosition = Vector3.zero;
    private GameSettings _gameSettings;
    
    // Public access to ground tiles for enemy spawn system
    public List<GameObject> GroundTiles => _groundTiles;

    [Inject]
    private void Construct(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
    }

    protected override Task Initialize()
    {
        try
        {
            SubscribeToEvents();
            ManageGroundTiles(_gameSettings.MapLength, false);
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
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<ContinueGameEvent>(OnContinueGame);
    }

    private void OnContinueGame(ContinueGameEvent continueGameEvent)
    {
        ManageGroundTiles(_gameSettings.MapLength, false);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    #endregion

    private void ManageGroundTiles(int requiredCount, bool isRestart)
    {
        //Remove nulls
        _groundTiles.RemoveAll(tile => tile == null);

        SpawnMissingTiles(requiredCount);

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
        for (int i = 0; i < _groundTiles.Count; i++)
        {
            if (_groundTiles[i] != null)
            {
                Vector3 newTilePosition =
                    repositionStartPosition + Vector3.forward * (i * _gameSettings.DistanceBetweenTiles);
                _groundTiles[i].transform.position = newTilePosition;
            }
        }
    }

    private Vector3 GetRepositionStartPositionAdvanced(bool isRestart)
    {
        float maxZ = float.MinValue;
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
        else
        {
            return _startPosition;
        }
    }
}