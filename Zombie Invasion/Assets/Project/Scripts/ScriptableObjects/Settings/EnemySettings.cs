using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Game/Enemy Settings")]
public class EnemySettings : ScriptableObject
{
    [Header("AI Settings")]
    public float aggroRadius = 10f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;

    [Header("Combat Settings")]
    public int damage = 25;
    public int maxHealth = 100;
}
