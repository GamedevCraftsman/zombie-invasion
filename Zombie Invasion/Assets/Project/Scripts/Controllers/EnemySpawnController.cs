using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class EnemySpawnController : BaseController
{
    private EnemySpawnSettings _settings;
    private IPool<EnemyController> _enemyPool;
    private SpawnMapManager _mapManager;

    //Move to ScriptableObject
    private readonly int _attemptsMultiplier = 3;
    //=========================

    private readonly List<Vector3> _allSpawnPoints = new();
    private readonly List<Vector3> _availableSpawnPoints = new();

    public List<Vector3> AllSpawnPoints => _allSpawnPoints;

    [Inject]
    private void Construct(SpawnMapManager mapManager, EnemySpawnSettings settings, IPool<EnemyController> enemyPool)
    {
        _settings = settings;
        _mapManager = mapManager;
        _enemyPool = enemyPool;
    }

    protected override Task Initialize()
    {
        try
        {
            GenerateAllSpawnPoints();
            SubscribeToEvents();
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
        EventBus.Subscribe<ReadyGameEvent>(OnGameReady);
        EventBus.Subscribe<RestarGameEvent>(OnGameRestart);
        EventBus.Subscribe<ContinueGameEvent>(OnContinueGame);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<RestarGameEvent>(OnGameRestart);
        EventBus?.Unsubscribe<ReadyGameEvent>(OnGameReady);
        EventBus?.Unsubscribe<ContinueGameEvent>(OnContinueGame);
    }

    private void OnGameReady(ReadyGameEvent readyGameEvent)
    {
        SpawnInitialEnemies();
    }

    private void OnGameRestart(RestarGameEvent gameRestartEvent)
    {
        //_usedSpawnPositions.Clear();
        GenerateAllSpawnPoints();
    }

    private void OnContinueGame(ContinueGameEvent gameContinueEvent)
    {
        // _usedSpawnPositions.Clear();
        GenerateAllSpawnPoints();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    #endregion

    private void GenerateAllSpawnPoints()
    {
        ResetPoints();
        CreatePoints();
    }

    private void ResetPoints()
    {
        _allSpawnPoints.Clear();
        _availableSpawnPoints.Clear();
    }

    private void CreatePoints()
    {
        if (_mapManager != null && _mapManager.GroundTiles.Count > 0)
        {
            GeneratePointsFromTiles();

            _availableSpawnPoints.AddRange(_allSpawnPoints);
        }
    }

    #region Generate spawn point

    private void GeneratePointsFromTiles()
    {
        var tiles = _mapManager.GroundTiles;
        int pointsPerTile =
            Mathf.CeilToInt((float)_settings.SpawnPointCount / (tiles.Count - _settings.CountIgnoreTiles()));

        HashSet<Vector3> uniquePoints = new HashSet<Vector3>();

        for (int tileIndex = _settings.StartTile(); CanGoToNextTile(tileIndex, uniquePoints, tiles); tileIndex++)
        {
            if (tiles[tileIndex] == null) continue;

            SpawnPointOnTile(pointsPerTile, uniquePoints, tiles, tileIndex);
        }

        _allSpawnPoints.AddRange(uniquePoints);
    }

    private bool CanGoToNextTile(int tileIndex, HashSet<Vector3> uniquePoints, List<GameObject> tiles)
    {
        return tileIndex < _settings.TilesWithoutLast(tiles.Count) && IsAllPointsSpawned(uniquePoints);
    }

    private void SpawnPointOnTile(int pointsPerTile, HashSet<Vector3> uniquePoints, List<GameObject> tiles,
        int tileIndex)
    {
        Vector3 tileCenter = tiles[tileIndex].transform.position;
        int attempts = 0;
        int maxAttemptsPerTile = pointsPerTile * _attemptsMultiplier;

        for (int pointIndex = 0;
             CanTryMakeNextPoint(pointIndex, pointsPerTile, uniquePoints, attempts, maxAttemptsPerTile);
             attempts++)
        {
            MakePoint(tileCenter, ref uniquePoints, ref pointIndex);
        }
    }

    #endregion

    #region CanTryMakeNextPoint

    private bool CanTryMakeNextPoint(int pointIndex, int pointsPerTile, HashSet<Vector3> uniquePoints, int attempts,
        int maxAttemptsPerTile)
    {
        return IsAllTilePointsSpawned(pointIndex, pointsPerTile) &&
               IsAllPointsSpawned(uniquePoints) &&
               HasAttempts(attempts, maxAttemptsPerTile);
    }

    private bool IsAllTilePointsSpawned(int pointIndex, int pointsPerTile)
    {
        return pointIndex < pointsPerTile;
    }

    private bool IsAllPointsSpawned(HashSet<Vector3> uniquePoints)
    {
        return uniquePoints.Count < _settings.SpawnPointCount;
    }

    private bool HasAttempts(int attempts, int maxAttemptsPerTile)
    {
        return attempts < maxAttemptsPerTile;
    }

    #endregion

    #region Make point methods

    private void MakePoint(Vector3 tileCenter, ref HashSet<Vector3> uniquePoints, ref int pointIndex)
    {
        if (CanAddPoint(tileCenter, ref uniquePoints))
        {
            pointIndex++;
        }
    }

    private bool CanAddPoint(Vector3 tileCenter, ref HashSet<Vector3> uniquePoints)
    {
        Vector3 tempSpawnPoint = RandomPointOnTile(tileCenter);

        return IsPositionValid(tempSpawnPoint, uniquePoints) &&
               uniquePoints.Add(tempSpawnPoint);
    }

    private Vector3 RandomPointOnTile(Vector3 tileCenter)
    {
        float offsetX = Random.Range(-_settings.SideXOffsetRange, _settings.SideXOffsetRange);
        float offsetZ = Random.Range(-_settings.SideZOffsetRange, _settings.SideZOffsetRange);
        Vector3 spawnPoint = new Vector3(
            Mathf.Round((tileCenter.x + offsetX) * 100f) / 100f,
            tileCenter.y,
            Mathf.Round((tileCenter.z + offsetZ) * 100f) / 100f
        );

        return spawnPoint;
    }

    private bool IsPositionValid(Vector3 position, HashSet<Vector3> uniquePoints)
    {
        float minDistance = _settings.MinSpawnDistance;

        foreach (Vector3 usedPosition in uniquePoints)
        {
            if (Vector3.Distance(position, usedPosition) < minDistance)
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    private void SpawnInitialEnemies()
    {
        _availableSpawnPoints.Clear();
        _availableSpawnPoints.AddRange(_allSpawnPoints);

        int enemiesToSpawn =
            Mathf.Min(_settings.TotalEnemyCount, _allSpawnPoints.Count, _settings.EnemyPoolInitialSize);
        Debug.LogWarning(enemiesToSpawn);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 spawnPosition = _allSpawnPoints[i];

            if (spawnPosition != Vector3.zero)
            {
                var enemy = _enemyPool.Get();
                enemy.transform.position = spawnPosition;
            }
            else
            {
                Debug.LogWarning($"Не вдалося знайти валідну позицію для спавну ворога {i + 1}");
                break;
            }
        }
    }


    #region Debug

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_allSpawnPoints == null || _allSpawnPoints.Count == 0)
            return;

        // Налаштуйте колір і розмір сфери за бажанням
        Gizmos.color = Color.green;
        float gizmoSize = 0.05f;

        foreach (var point in _allSpawnPoints)
        {
            Gizmos.DrawWireSphere(point, gizmoSize);
        }
    }
#endif

    #endregion
}