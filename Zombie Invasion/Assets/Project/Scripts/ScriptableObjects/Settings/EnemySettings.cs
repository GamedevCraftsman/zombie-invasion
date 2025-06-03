using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Game/Enemy Settings")]
public class EnemySettings : ScriptableObject
{
    [Header("AI Settings")]
    [SerializeField] private float aggroRadius = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Combat Settings")]
    [SerializeField] private int damage = 25;
    [SerializeField] private int maxHealth = 100;

    #region Public Values

    public float AggroRadius => aggroRadius;
    public float MoveSpeed => moveSpeed;
    public float RotationSpeed => rotationSpeed;
    public int Damage => damage;
    public int MaxHealth => maxHealth;

    #endregion
}
