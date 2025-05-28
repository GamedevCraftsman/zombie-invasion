using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSettings", menuName = "Game/Weapon Settings")]
public class WeaponSettings : ScriptableObject
{
    [Header("Shooting")]
    public float fireRate = 0.5f;
    public float bulletSpeed = 20f;
    public float bulletDamage = 25f;
    public float bulletLifetime = 5f;
    
    [Header("Turret")]
    public float turretRotationSpeed = 90f;
    public float maxRotationAngle = 45f;
}