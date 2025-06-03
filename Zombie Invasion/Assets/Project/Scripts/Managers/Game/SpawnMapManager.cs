using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnMapManager : BaseManager
{
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private GameObject groundTilesContainer;

    private List<GameObject> groundTiles = new List<GameObject>();
    private Vector3 startPosition = Vector3.zero;

    // Public access to ground tiles for enemy spawn system
    public List<GameObject> GroundTiles => groundTiles;

    protected override Task Initialize()
    {
        try
        {
            SubscribeToEvents();
            ManageGroundTiles(gameSettings.MapLength, false);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return Task.CompletedTask;
    }

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
        ManageGroundTiles(gameSettings.MapLength, false);
    }

    public void ManageGroundTiles(int requiredCount, bool isRestart)
    {
        //Remove nulls
        groundTiles.RemoveAll(tile => tile == null);

        SpawnMissingTiles(requiredCount);

        RepositionAllTiles(isRestart);
    }

    private void SpawnMissingTiles(int requiredCount)
    {
        int currentCount = groundTiles.Count;

        if (currentCount < requiredCount)
        {
            int tilesToSpawn = requiredCount - currentCount;

            for (int i = 0; i < tilesToSpawn; i++)
            {
                GameObject newTile = Instantiate(gameSettings.MapTilePrefab, groundTilesContainer.transform);
                groundTiles.Add(newTile);
            }
        }
    }

    private void RepositionAllTiles(bool isRestart)
    {
        Vector3 repositionStartPosition = GetRepositionStartPositionAdvanced(isRestart);

        // Move to new positions
        for (int i = 0; i < groundTiles.Count; i++)
        {
            if (groundTiles[i] != null)
            {
                Vector3 newTilePosition =
                    repositionStartPosition + Vector3.forward * (i * gameSettings.DistanceBetweenTiles);
                groundTiles[i].transform.position = newTilePosition;
            }
        }
    }

    private Vector3 GetRepositionStartPositionAdvanced(bool isRestart)
    {
        float maxZ = float.MinValue;
        bool foundAnyTile = false;

        foreach (GameObject tile in groundTiles)
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
            return new Vector3(startPosition.x, startPosition.y, maxZ);
        }
        else
        {
            return startPosition;
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}