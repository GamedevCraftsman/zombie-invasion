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
    //private readonly HashSet<Vector3> _usedSpawnPositions = new();

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

            //ShuffleSpawnPoints();

            _availableSpawnPoints.AddRange(_allSpawnPoints);
        }
    }

    //========================================================================================================================================================================
    private void GeneratePointsFromTiles()
    {
        var tiles = _mapManager.GroundTiles;
        int pointsPerTile = Mathf.CeilToInt((float)_settings.SpawnPointCount / (tiles.Count - _settings.CountIgnoreTiles()));
        Debug.LogWarning(pointsPerTile);
        
        HashSet<Vector3> uniquePoints = new HashSet<Vector3>();

        for (int tileIndex = _settings.StartTile(); tileIndex < _settings.TilesWithoutLast(tiles.Count) && uniquePoints.Count < _settings.SpawnPointCount; tileIndex++)
        {
            if (tiles[tileIndex] == null) continue;

            Vector3 tileCenter = tiles[tileIndex].transform.position;
            int attempts = 0;
            int maxAttemptsPerTile = pointsPerTile * _attemptsMultiplier;

            for (int pointIndex = 0;
                 pointIndex < pointsPerTile && 
                 uniquePoints.Count < _settings.SpawnPointCount &&
                 attempts < maxAttemptsPerTile;
                 attempts++)
            {
                float offsetX = Random.Range(-_settings.SideXOffsetRange, _settings.SideXOffsetRange);
                float offsetZ = Random.Range(-_settings.SideZOffsetRange, _settings.SideZOffsetRange);
                Vector3 spawnPoint = new Vector3(
                    Mathf.Round((tileCenter.x + offsetX) * 100f) / 100f,
                    tileCenter.y,
                    Mathf.Round((tileCenter.z + offsetZ) * 100f) / 100f
                );

                if (IsPositionValid(spawnPoint, uniquePoints) && uniquePoints.Add(spawnPoint))
                {
                    pointIndex++;
                }
            }
        }

        _allSpawnPoints.AddRange(uniquePoints);
    }

    
    //========================================================================================================================================================================
    // private void ShuffleSpawnPoints()
    // {
    //     for (int i = 0; i < _allSpawnPoints.Count; i++)
    //     {
    //         Vector3 temp = _allSpawnPoints[i];
    //         int randomIndex = Random.Range(i, _allSpawnPoints.Count);
    //         _allSpawnPoints[i] = _allSpawnPoints[randomIndex];
    //         _allSpawnPoints[randomIndex] = temp;
    //     }
    // }


    private void SpawnInitialEnemies()
    {
        //_usedSpawnPositions.Clear();
        _availableSpawnPoints.Clear();
        _availableSpawnPoints.AddRange(_allSpawnPoints);

        // int enemiesToSpawn = Mathf.Min(_settings.TotalEnemyCount, _allSpawnPoints.Count);
        // Debug.LogWarning(enemiesToSpawn);
        
        int enemiesToSpawn = Mathf.Min(_settings.TotalEnemyCount, _allSpawnPoints.Count, _settings.EnemyPoolInitialSize);
        Debug.LogWarning(enemiesToSpawn);
        
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 spawnPosition = _allSpawnPoints[i];/*GetValidSpawnPosition();*/

            if (spawnPosition != Vector3.zero)
            {
                var enemy = _enemyPool.Get();
                enemy.transform.position = spawnPosition;
               // _usedSpawnPositions.Add(spawnPosition);
            }
            else
            {
                Debug.LogWarning($"Не вдалося знайти валідну позицію для спавну ворога {i + 1}");
                break;
            }
        }
    }

    /*private Vector3 GetValidSpawnPosition()
    {
        const int maxAttempts = 100;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (_availableSpawnPoints.Count == 0)
            {
                Debug.LogWarning("Немає доступних точок спавну!");
                return Vector3.zero;
            }

            int randomIndex = Random.Range(0, _availableSpawnPoints.Count);
            Vector3 candidatePosition = _availableSpawnPoints[randomIndex];

            if (IsPositionValid(candidatePosition))
            {
                _availableSpawnPoints.RemoveAt(randomIndex);
                return candidatePosition;
            }

            _availableSpawnPoints.RemoveAt(randomIndex);
        }

        Debug.LogWarning("Не вдалося знайти валідну позицію після максимальної кількості спроб");
        return Vector3.zero;
    }*/

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

    // public Vector3 GetNextAvailableSpawnPoint()
    // {
    //     return GetValidSpawnPosition();
    // }
    
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

}