using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSettings", menuName = "Game/Weapon Settings")]
public class WeaponSettings : ScriptableObject
{
    [Header("Shooting")]
    public float fireRate = 0.5f;
    public float bulletSpeed = 20f;
    public int bulletDamage = 25;
    public float bulletLifetime = 5f;
    public GameObject bulletPrefab;
    
    [Header("Bullet Pool")]
    [Range(5, 20)]
    public int poolSize = 10;
    
    [Header("Turret Rotation")]
    [Range(30f, 180f)]
    public float maxRotationAngle = 90f;
    
    [Range(150, 250)]
    public float rotationSpeed = 50f;
    
    [Header("Input Sensitivity")]
    [Range(10, 30)]
    public float inputSensitivity = 1f;
}