using UnityEngine;

public interface IEnemyManager
{
    void SpawnEnemies();
    void RegisterEnemyDeath(Vector3 position);
    int EnemiesAlive { get; }
    event System.Action<int> OnEnemyCountChanged;
}