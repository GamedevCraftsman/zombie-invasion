using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnSettings", menuName = "Game/Enemy Spawn Settings")]
public class EnemySpawnSettings : ScriptableObject
{
    [Header("Enemy Settings")]
    public GameObject EnemyPrefab;
    
    [Header("Spawn Count Settings")]
    public int TotalEnemyCount = 50;
    public int SpawnPointCount = 100;
    public int EnemyPoolInitialSize = 60;
    
    [Header("Spawn Area Settings")]
    public Vector3 SpawnAreaCenter = Vector3.zero;
    //public Vector3 SpawnAreaSize = new Vector3(10f, 0f, 100f);
    public float SideOffsetRange = 2f;
    
    [Header("Advanced Settings")]
    public float MinSpacing = 1f;
    public float DistanceBetweenSpawnPoints = 1f;
}