using System.Collections.Generic;
using UnityEngine;

public interface IEnemySpawner
{
    void SpawnEnemies(List<Vector3> spawnPoints, IPool<EnemyController> pool, int totalCount, int poolSize);
}