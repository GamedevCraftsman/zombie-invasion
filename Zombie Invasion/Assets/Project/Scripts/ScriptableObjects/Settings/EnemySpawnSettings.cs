using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnSettings", menuName = "Game/Enemy Spawn Settings")]
public class EnemySpawnSettings : ScriptableObject
{
    [Header("Enemy Settings")] 
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Count Settings")] 
    [SerializeField] private GameSettings gameSettings;

    [SerializeField, Min(0)] private int totalEnemyCount = 50;

    [SerializeField, Min(0)] private int spawnPointCount = 100;
    [SerializeField, Min(1)] private int enemyPoolInitialSize = 60;

    [Header("Spawn Area Settings")] 
    [SerializeField, Range(0, 1)] private float sideXOffsetRange = 1f;

    [SerializeField, Range(0, 2)] private float sideZOffsetRange = 2f;

    [Header("Spawn distance")] 
    [SerializeField] private float minSpawnDistance = 2f;

    [SerializeField] private bool ignoreFirstTile;
    [SerializeField] private bool ignoreLastTile;

    #region Public Properties

    public GameObject EnemyPrefab => enemyPrefab;
    public int TotalEnemyCount => totalEnemyCount;
    public int SpawnPointCount => spawnPointCount;
    public int EnemyPoolInitialSize => enemyPoolInitialSize;
    public float SideXOffsetRange => sideXOffsetRange;
    public float SideZOffsetRange => sideZOffsetRange;
    public float MinSpawnDistance => minSpawnDistance;

    #endregion

    #region Public Methods

    public int CountIgnoreTiles()
    {
        int count = 0;

        if (ignoreFirstTile) count++;
        if (ignoreLastTile) count++;

        return count;
    }

    public int TilesWithoutLast(int tileCount)
    {
        if (ignoreLastTile) return tileCount - 1;

        return tileCount;
    }

    public int StartTile()
    {
        if (ignoreFirstTile) return 1;
        return 0;
    }

    #endregion

    private void OnValidate()
    {
        MakePoolLessTotalEnemyCount();
        SetMinPoolSize();

        spawnPointCount = totalEnemyCount;
    }

    #region Private Methods

    private void MakePoolLessTotalEnemyCount()
    {
        if (totalEnemyCount < enemyPoolInitialSize)
        {
            enemyPoolInitialSize = totalEnemyCount;
        }
    }

    private void SetMinPoolSize()
    {
        if (enemyPoolInitialSize < EnemiesForTwoTiles())
        {
            enemyPoolInitialSize = EnemiesForTwoTiles();
        }
    }

    private int EnemiesForTwoTiles()
    {
        return Mathf.CeilToInt((float)spawnPointCount / (gameSettings.MapLength - CountIgnoreTiles())) * 2;
    }

    #endregion
}