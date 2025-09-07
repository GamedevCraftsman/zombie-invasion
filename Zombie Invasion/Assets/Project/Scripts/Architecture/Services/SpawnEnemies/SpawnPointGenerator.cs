using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;

public class SpawnPointGenerator : ISpawnPointGenerator
{
    private readonly IProgressIncreaseService _progressIncreaseService;
    private readonly int _attemptsMultiplier = 3;
    private readonly ISpawnPointValidator _validator;
    private readonly EnemySpawnSettings _spawnSettings;
    
    private int _spawnEnemyCount;
    [Inject]
    public SpawnPointGenerator(ISpawnPointValidator validator, IProgressIncreaseService progressIncreaseService, EnemySpawnSettings spawnSettings)
    {
        _validator = validator;
        _spawnSettings = spawnSettings;
        _progressIncreaseService = progressIncreaseService;
    }

    public List<Vector3> GeneratePoints(List<GameObject> tiles)
    {
        _spawnEnemyCount = _spawnSettings.TotalEnemyCount + _progressIncreaseService.EnemiesIncrease;
        var uniquePoints = new HashSet<Vector3>();
        var validTiles = GetValidTiles(tiles).ToList();
        int pointsPerTile = CalculatePointsPerTile(validTiles.Count);

        foreach (var tile in validTiles)
        {
            if (HasReachedSpawnLimit(uniquePoints.Count, _spawnEnemyCount))
                break;

            GeneratePointsForTile(tile, pointsPerTile, uniquePoints);
        }

        return ConvertToList(uniquePoints);
    }

    private IEnumerable<GameObject> GetValidTiles(List<GameObject> tiles)
    {
        return tiles.Skip(_spawnSettings.StartTile())
            .Take(GetTileRangeCount(tiles.Count))
            .Where(tile => tile != null);
    }

    private int GetTileRangeCount(int totalTiles)
    {
        return _spawnSettings.TilesWithoutLast(totalTiles) - _spawnSettings.StartTile();
    }

    private int CalculatePointsPerTile(int validTileCount)
    {
        return Mathf.CeilToInt((float)_spawnEnemyCount / validTileCount);
    }

    private bool HasReachedSpawnLimit(int currentCount, int maxCount)
    {
        return currentCount >= maxCount;
    }

    private void GeneratePointsForTile(GameObject tile, int pointsPerTile, HashSet<Vector3> uniquePoints)
    {
        var center = tile.transform.position;
        var maxAttempts = CalculateMaxAttempts(pointsPerTile);
        var targetPoints = CalculateTargetPointsForTile(pointsPerTile, uniquePoints.Count, _spawnEnemyCount);

        TryGeneratePointsWithAttempts(center, targetPoints, maxAttempts, uniquePoints);
    }

    private int CalculateMaxAttempts(int pointsPerTile)
    {
        return pointsPerTile * _attemptsMultiplier;
    }

    private int CalculateTargetPointsForTile(int pointsPerTile, int currentPointCount, int totalSpawnLimit)
    {
        return Mathf.Min(pointsPerTile, totalSpawnLimit - currentPointCount);
    }

    private void TryGeneratePointsWithAttempts(Vector3 center, int targetPoints, int maxAttempts,
        HashSet<Vector3> uniquePoints)
    {
        int spawned = 0;
        int attempts = 0;

        while (ShouldContinueGenerating(spawned, targetPoints, attempts, maxAttempts, uniquePoints.Count,
                   _spawnEnemyCount))
        {
            var point = RandomPointOnTile(center);

            if (TryAddValidPoint(point, uniquePoints))
            {
                spawned++;
            }

            attempts++;
        }
    }

    private bool ShouldContinueGenerating(int spawned, int targetPoints, int attempts, int maxAttempts, int totalPoints,
        int spawnLimit)
    {
        return spawned < targetPoints &&
               attempts < maxAttempts &&
               totalPoints < spawnLimit;
    }

    private bool TryAddValidPoint(Vector3 point, HashSet<Vector3> uniquePoints)
    {
        if (_validator.IsValid(point, uniquePoints, _spawnSettings.MinSpawnDistance))
        {
            uniquePoints.Add(point);
            return true;
        }

        return false;
    }

    private List<Vector3> ConvertToList(HashSet<Vector3> uniquePoints)
    {
        return new List<Vector3>(uniquePoints);
    }
    
    private Vector3 RandomPointOnTile(Vector3 center)
    {
        float offsetX = Random.Range(-_spawnSettings.SideXOffsetRange, _spawnSettings.SideXOffsetRange);
        float offsetZ = Random.Range(-_spawnSettings.SideZOffsetRange, _spawnSettings.SideZOffsetRange);
        return new Vector3(
            Mathf.Round((center.x + offsetX) * 100f) / 100f,
            center.y,
            Mathf.Round((center.z + offsetZ) * 100f) / 100f
        );
    }
}