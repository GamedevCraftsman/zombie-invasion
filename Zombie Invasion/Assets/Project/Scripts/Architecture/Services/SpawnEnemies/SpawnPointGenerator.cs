using System.Collections.Generic;
using UnityEngine;

public class SpawnPointGenerator : ISpawnPointGenerator
{
    private readonly int _attemptsMultiplier = 3;
    private readonly ISpawnPointValidator _validator;

    public SpawnPointGenerator(ISpawnPointValidator validator)
    {
        _validator = validator;
    }

    public List<Vector3> GeneratePoints(List<GameObject> tiles, EnemySpawnSettings settings)
    {
        var uniquePoints = new HashSet<Vector3>();
        int validTileCount = tiles.Count - settings.CountIgnoreTiles();
        int pointsPerTile = Mathf.CeilToInt((float)settings.SpawnPointCount / validTileCount);

        for (int i = settings.StartTile(); i < settings.TilesWithoutLast(tiles.Count); i++)
        {
            var tile = tiles[i];
            if (tile == null) continue;

            Vector3 center = tile.transform.position;
            int attempts = 0;
            int maxAttempts = pointsPerTile * _attemptsMultiplier;
            int spawned = 0;

            while (spawned < pointsPerTile && attempts < maxAttempts && uniquePoints.Count < settings.SpawnPointCount)
            {
                var point = RandomPointOnTile(center, settings);
                if (_validator.IsValid(point, uniquePoints, settings.MinSpawnDistance))
                {
                    uniquePoints.Add(point);
                    spawned++;
                }
                attempts++;
            }
        }

        return new List<Vector3>(uniquePoints);
    }

    private Vector3 RandomPointOnTile(Vector3 center, EnemySpawnSettings settings)
    {
        float offsetX = UnityEngine.Random.Range(-settings.SideXOffsetRange, settings.SideXOffsetRange);
        float offsetZ = UnityEngine.Random.Range(-settings.SideZOffsetRange, settings.SideZOffsetRange);
        return new Vector3(
            Mathf.Round((center.x + offsetX) * 100f) / 100f,
            center.y,
            Mathf.Round((center.z + offsetZ) * 100f) / 100f
        );
    }
}