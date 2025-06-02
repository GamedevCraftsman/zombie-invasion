using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class EnemySpawnController : BaseController
{
    [Inject] private EnemySpawnSettings _settings;
    [Inject] private IPool<EnemyController> _enemyPool;
    [Inject] private SpawnMapManager _mapManager;
   // [Inject] private IEventBus _eventBus;
    
    private List<Vector3> _allSpawnPoints = new List<Vector3>();
    private bool _isGameStarted = false;
    
    public List<Vector3> AllSpawnPoints => _allSpawnPoints;

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
    
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<ReadyGameEvent>(OnGameReady);
        EventBus.Subscribe<RestarGameEvent>(OnGameRestart);
        EventBus.Subscribe<ContinueGameEvent>(OnContinueGame);
    }
    
    private void OnDestroy()
    {
        EventBus?.Unsubscribe<RestarGameEvent>(OnGameRestart);
        EventBus?.Unsubscribe<ReadyGameEvent>(OnGameReady);
        EventBus?.Subscribe<ContinueGameEvent>(OnContinueGame);
    }
    
    private void GenerateAllSpawnPoints()
    {
        _allSpawnPoints.Clear();
        
        if (_mapManager != null && _mapManager.GroundTiles.Count > 0)
        {
            GeneratePointsFromTiles();
        }
        // else
        // {
        //     GeneratePointsFromArea();
        // }
    }
    
    private void GeneratePointsFromTiles()
    {
        var tiles = _mapManager.GroundTiles;
        int pointsPerTile = Mathf.CeilToInt((float)_settings.SpawnPointCount / tiles.Count);
        
        for (int tileIndex = 1; tileIndex < tiles.Count && _allSpawnPoints.Count < _settings.SpawnPointCount; tileIndex++)
        {
            if (tiles[tileIndex] == null) continue;
            
            Vector3 tileCenter = tiles[tileIndex].transform.position;
            
            for (int pointIndex = 0; pointIndex < pointsPerTile && _allSpawnPoints.Count < _settings.SpawnPointCount; pointIndex++)
            {
                float offsetX = Random.Range(-_settings.SideXOffsetRange, _settings.SideXOffsetRange);
                float offsetZ = Random.Range(-_settings.SideZOffsetRange, _settings.SideZOffsetRange);
                Vector3 spawnPoint = new Vector3(
                    tileCenter.x + offsetX,
                    tileCenter.y,
                    tileCenter.z + offsetZ
                );
                _allSpawnPoints.Add(spawnPoint);
            }
        }
    }
    
    /*private void GeneratePointsFromArea()
    {
        for (int i = 0; i < _settings.SpawnPointCount; i++)
        {
            float t = i / (float)(_settings.SpawnPointCount - 1);
            Vector3 centerPos = _settings.SpawnAreaCenter + Vector3.forward * (t * _settings.SpawnAreaSize.z);
            float offsetX = Random.Range(-_settings.SideOffsetRange, _settings.SideOffsetRange);
            
            Vector3 spawnPoint = new Vector3(
                centerPos.x + offsetX,
                centerPos.y,
                centerPos.z
            );
            _allSpawnPoints.Add(spawnPoint);
        }
    }*/
    
    private void OnGameReady(ReadyGameEvent readyGameEvent)
    {
        //if (_isGameStarted) return;
        
        _isGameStarted = true;
        SpawnInitialEnemies();
    }

    private void OnGameRestart(RestarGameEvent gameRestartEvent)
    {
        GenerateAllSpawnPoints();
    }

    private void OnContinueGame(ContinueGameEvent gameContinueEvent)
    {
        GenerateAllSpawnPoints();
    }
    
    private void SpawnInitialEnemies()
    {
        int enemiesToSpawn = Mathf.Min(_settings.TotalEnemyCount, _allSpawnPoints.Count);
        
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            var enemy = _enemyPool.Get();
            enemy.transform.position = _allSpawnPoints[i];
        }
        
        Debug.Log($"Spawned {enemiesToSpawn} enemies at initial positions");
    }
}