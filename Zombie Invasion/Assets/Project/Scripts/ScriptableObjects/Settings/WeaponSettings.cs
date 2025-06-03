using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSettings", menuName = "Game/WeaponSettings")]
public class WeaponSettings : ScriptableObject
{
    [Header("Bullet Properties")] [SerializeField]
    private float fireRate = 0.5f;

    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private int bulletDamage = 25;
    [SerializeField] private float bulletLifetime = 5f;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Bullet Pool")] [SerializeField, Range(5, 20)]
    private int poolSize = 10;

    [Header("Turret Rotation")] [SerializeField, Range(30f, 180f)]
    private float maxRotationAngle = 90f;

    [SerializeField, Range(50, 150)] private float rotationSpeed = 50f;

    [Header("Input Sensitivity")] [SerializeField, Range(0.1f, 10f)]
    private float inputSensitivity = 1f;

    #region Public Values

    public float FireRate => fireRate;
    public float BulletSpeed => bulletSpeed;
    public int BulletDamage => bulletDamage;
    public float BulletLifetime => bulletLifetime;
    public GameObject BulletPrefab => bulletPrefab;

    public int PoolSize => poolSize;

    public float MaxRotationAngle => maxRotationAngle;
    public float RotationSpeed => rotationSpeed;

    public float InputSensitivity => inputSensitivity;

    #endregion
}