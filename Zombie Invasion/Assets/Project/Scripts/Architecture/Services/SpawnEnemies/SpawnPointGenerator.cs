using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;

public class SpawnPointGenerator : ISpawnPointGenerator
{
    private readonly int _attemptsMultiplier = 3;
    private readonly ISpawnPointValidator _validator;

    [Inject]
    public SpawnPointGenerator(ISpawnPointValidator validator)
    {
        _validator = validator;
    }

    public List<Vector3> GeneratePoints(List<GameObject> tiles, EnemySpawnSettings settings)
    {
        var uniquePoints = new HashSet<Vector3>();
        var validTiles = GetValidTiles(tiles, settings).ToList();
        int pointsPerTile = CalculatePointsPerTile(validTiles.Count, settings);

        foreach (var tile in validTiles)
        {
            if (HasReachedSpawnLimit(uniquePoints.Count, settings.SpawnPointCount))
                break;

            GeneratePointsForTile(tile, pointsPerTile, uniquePoints, settings);
        }

        return ConvertToList(uniquePoints);
    }

    private IEnumerable<GameObject> GetValidTiles(List<GameObject> tiles, EnemySpawnSettings settings)
    {
        return tiles.Skip(settings.StartTile())
            .Take(GetTileRangeCount(tiles.Count, settings))
            .Where(tile => tile != null);
    }

    private int GetTileRangeCount(int totalTiles, EnemySpawnSettings settings)
    {
        return settings.TilesWithoutLast(totalTiles) - settings.StartTile();
    }

    private int CalculatePointsPerTile(int validTileCount, EnemySpawnSettings settings)
    {
        return Mathf.CeilToInt((float)settings.SpawnPointCount / validTileCount);
    }

    private bool HasReachedSpawnLimit(int currentCount, int maxCount)
    {
        return currentCount >= maxCount;
    }

    private void GeneratePointsForTile(GameObject tile, int pointsPerTile, HashSet<Vector3> uniquePoints,
        EnemySpawnSettings settings)
    {
        var center = tile.transform.position;
        var maxAttempts = CalculateMaxAttempts(pointsPerTile);
        var targetPoints = CalculateTargetPointsForTile(pointsPerTile, uniquePoints.Count, settings.SpawnPointCount);

        TryGeneratePointsWithAttempts(center, targetPoints, maxAttempts, uniquePoints, settings);
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
        HashSet<Vector3> uniquePoints, EnemySpawnSettings settings)
    {
        int spawned = 0;
        int attempts = 0;

        while (ShouldContinueGenerating(spawned, targetPoints, attempts, maxAttempts, uniquePoints.Count,
                   settings.SpawnPointCount))
        {
            var point = RandomPointOnTile(center, settings);

            if (TryAddValidPoint(point, uniquePoints, settings))
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

    private bool TryAddValidPoint(Vector3 point, HashSet<Vector3> uniquePoints, EnemySpawnSettings settings)
    {
        if (_validator.IsValid(point, uniquePoints, settings.MinSpawnDistance))
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
    
    private Vector3 RandomPointOnTile(Vector3 center, EnemySpawnSettings settings)
    {
        float offsetX = Random.Range(-settings.SideXOffsetRange, settings.SideXOffsetRange);
        float offsetZ = Random.Range(-settings.SideZOffsetRange, settings.SideZOffsetRange);
        return new Vector3(
            Mathf.Round((center.x + offsetX) * 100f) / 100f,
            center.y,
            Mathf.Round((center.z + offsetZ) * 100f) / 100f
        );
    }
}