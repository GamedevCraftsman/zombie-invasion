using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnSettings", menuName = "Game/Enemy Spawn Settings")]
public class EnemySpawnSettings : ScriptableObject
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Count Settings")]
    [SerializeField, Min(0)] private int totalEnemyCount = 50;
    [SerializeField, Min(0)] private int spawnPointCount = 100;
    [SerializeField, Min(1)] private int enemyPoolInitialSize = 60;

    [Header("Spawn Area Settings")]
    [SerializeField, Min(0f)] private float sideXOffsetRange = 2f;
    [SerializeField, Min(0f)] private float sideZOffsetRange = 2f;

    [Header("Sapwn distance")]
    [SerializeField] private float _minSpawnDistance = 2f;

    #region Public Properties
    
    public GameObject EnemyPrefab => enemyPrefab;
    public int TotalEnemyCount => totalEnemyCount;
    public int SpawnPointCount => spawnPointCount;
    public int EnemyPoolInitialSize => enemyPoolInitialSize;
    public float SideXOffsetRange => sideXOffsetRange;
    public float SideZOffsetRange => sideZOffsetRange;
    public float MinSpawnDistance => _minSpawnDistance;
    
    #endregion
}