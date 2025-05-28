using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Game/Enemy Settings")]
public class EnemySettings : ScriptableObject
{
    [Header("Spawn")]
    public int maxEnemiesOnField = 5;
    public float spawnInterval = 2f;
    public float spawnDistance = 20f;
    
    [Header("Behavior")]
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float damage = 10f;
    public float health = 50f;
}
