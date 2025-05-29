using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSettings", menuName = "Game/Weapon Settings")]
public class WeaponSettings : ScriptableObject
{
    [Header("Shooting")]
    public float fireRate = 0.5f;
    public float bulletSpeed = 20f;
    public float bulletDamage = 25f;
    public float bulletLifetime = 5f;
    
    [Header("Turret Rotation")]
    [Range(30f, 180f)]
    public float maxRotationAngle = 90f;
    
    [Range(150, 250)]
    public float rotationSpeed = 50f;
    
    [Header("Input Sensitivity")]
    [Range(10, 30)]
    public float inputSensitivity = 1f;
}