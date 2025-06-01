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
    [SerializeField] private Vector3 spawnAreaCenter = Vector3.zero;
    [SerializeField, Min(0f)] private float sideXOffsetRange = 2f;
    [SerializeField, Min(0f)] private float sideZOffsetRange = 2f;

    [Header("Advanced Settings")]
    [SerializeField, Min(0f)] private float minSpacing = 1f;
    [SerializeField, Min(0f)] private float distanceBetweenSpawnPoints = 1f;


    #region Public Properties
    
    public GameObject EnemyPrefab => enemyPrefab;
    public int TotalEnemyCount => totalEnemyCount;
    public int SpawnPointCount => spawnPointCount;
    public int EnemyPoolInitialSize => enemyPoolInitialSize;
    public Vector3 SpawnAreaCenter => spawnAreaCenter;
    public float SideXOffsetRange => sideXOffsetRange;
    public float SideZOffsetRange => sideZOffsetRange;
    public float MinSpacing => minSpacing;
    public float DistanceBetweenSpawnPoints => distanceBetweenSpawnPoints;
    
    #endregion
}