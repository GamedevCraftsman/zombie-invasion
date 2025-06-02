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

    private List<Vector3> _allSpawnPoints = new List<Vector3>();
    private List<Vector3> _availableSpawnPoints = new List<Vector3>(); // Зберігаємо доступні точки
    private HashSet<Vector3> _usedSpawnPositions = new HashSet<Vector3>(); // Використовуємо HashSet для швидшого пошуку

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

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<RestarGameEvent>(OnGameRestart);
        EventBus?.Unsubscribe<ReadyGameEvent>(OnGameReady);
        EventBus?.Unsubscribe<ContinueGameEvent>(OnContinueGame);
    }

    private void GenerateAllSpawnPoints()
    {
        _allSpawnPoints.Clear();
        _availableSpawnPoints.Clear();

        if (_mapManager != null && _mapManager.GroundTiles.Count > 0)
        {
            GeneratePointsFromTiles();
            
            // Перемішуємо список для рандомності
            ShuffleSpawnPoints();
            
            // Копіюємо у доступні точки
            _availableSpawnPoints.AddRange(_allSpawnPoints);
        }
    }

    private void GeneratePointsFromTiles()
    {
        var tiles = _mapManager.GroundTiles;
        int pointsPerTile = Mathf.CeilToInt((float)_settings.SpawnPointCount / tiles.Count);
        HashSet<Vector3> uniquePoints = new HashSet<Vector3>(); // Для уникнення дублікатів

        for (int tileIndex = 1; tileIndex < tiles.Count && uniquePoints.Count < _settings.SpawnPointCount; tileIndex++)
        {
            if (tiles[tileIndex] == null) continue;

            Vector3 tileCenter = tiles[tileIndex].transform.position;
            int attempts = 0;
            int maxAttemptsPerTile = pointsPerTile * 3; // Збільшуємо кількість спроб

            for (int pointIndex = 0; 
                 pointIndex < pointsPerTile && uniquePoints.Count < _settings.SpawnPointCount && attempts < maxAttemptsPerTile; 
                 attempts++)
            {
                float offsetX = Random.Range(-_settings.SideXOffsetRange, _settings.SideXOffsetRange);
                float offsetZ = Random.Range(-_settings.SideZOffsetRange, _settings.SideZOffsetRange);
                Vector3 spawnPoint = new Vector3(
                    Mathf.Round((tileCenter.x + offsetX) * 100f) / 100f, // Округлюємо до 2 знаків після коми
                    tileCenter.y,
                    Mathf.Round((tileCenter.z + offsetZ) * 100f) / 100f
                );

                // Перевіряємо унікальність позиції
                if (uniquePoints.Add(spawnPoint))
                {
                    pointIndex++;
                }
            }
        }

        _allSpawnPoints.AddRange(uniquePoints);
        Debug.Log($"Generated {_allSpawnPoints.Count} unique spawn points");
    }

    private void ShuffleSpawnPoints()
    {
        for (int i = 0; i < _allSpawnPoints.Count; i++)
        {
            Vector3 temp = _allSpawnPoints[i];
            int randomIndex = Random.Range(i, _allSpawnPoints.Count);
            _allSpawnPoints[i] = _allSpawnPoints[randomIndex];
            _allSpawnPoints[randomIndex] = temp;
        }
    }

    private void OnGameReady(ReadyGameEvent readyGameEvent)
    {
        SpawnInitialEnemies();
    }

    private void OnGameRestart(RestarGameEvent gameRestartEvent)
    {
        _usedSpawnPositions.Clear();
        GenerateAllSpawnPoints();
    }

    private void OnContinueGame(ContinueGameEvent gameContinueEvent)
    {
        _usedSpawnPositions.Clear();
        GenerateAllSpawnPoints();
    }

    private void SpawnInitialEnemies()
    {
        _usedSpawnPositions.Clear();
        _availableSpawnPoints.Clear();
        _availableSpawnPoints.AddRange(_allSpawnPoints);
        
        int enemiesToSpawn = Mathf.Min(_settings.TotalEnemyCount, _allSpawnPoints.Count);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();
            
            if (spawnPosition != Vector3.zero)
            {
                var enemy = _enemyPool.Get();
                enemy.transform.position = spawnPosition;
                _usedSpawnPositions.Add(spawnPosition);
                
                Debug.Log($"Spawned enemy {i + 1} at position: {spawnPosition}");
            }
            else
            {
                Debug.LogWarning($"Не вдалося знайти валідну позицію для спавну ворога {i + 1}");
                break;
            }
        }
    }

    private Vector3 GetValidSpawnPosition()
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
                _availableSpawnPoints.RemoveAt(randomIndex); // Видаляємо використану позицію
                return candidatePosition;
            }
            
            // Видаляємо невалідну позицію зі списку доступних
            _availableSpawnPoints.RemoveAt(randomIndex);
        }

        Debug.LogWarning("Не вдалося знайти валідну позицію після максимальної кількості спроб");
        return Vector3.zero;
    }

    private bool IsPositionValid(Vector3 position)
    {
        float minDistance = _settings.MinSpawnDistance;
        
        foreach (Vector3 usedPosition in _usedSpawnPositions)
        {
            if (Vector3.Distance(position, usedPosition) < minDistance)
            {
                return false;
            }
        }

        return true;
    }

    public Vector3 GetNextAvailableSpawnPoint()
    {
        return GetValidSpawnPosition();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}