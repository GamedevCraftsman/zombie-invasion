using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : IEnemySpawner
{
    public void SpawnEnemies(List<Vector3> spawnPoints, IPool<EnemyController> pool, int totalCount, int poolSize)
    {
        int count = Mathf.Min(totalCount, spawnPoints.Count, poolSize);
        SetEnemiesAtPoints(count, spawnPoints, pool);
    }

    private void SetEnemiesAtPoints(int count, List<Vector3> spawnPoints, IPool<EnemyController> pool)
    {
        for (int i = 0; i < count; i++)
        {
            var position = spawnPoints[i];
            var enemy = pool.Get();
            enemy.transform.position = position;
        }
    }
}