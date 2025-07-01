using System;
using Zenject;
using System.Threading.Tasks;
using UnityEngine;

public class FireController : BaseController, IFireController
{
    [Header("References")] 
    [SerializeField] private Transform turretTransform;

    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletPool bulletPool;
    
    // Injected dependencies
    [Inject] private WeaponSettings _weaponSettings;
    
    private float lastFireTime = 0f;
    
    protected override Task Initialize()
    {
        try
        {
            ValidateComponents();
        }
        catch ( Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return Task.CompletedTask;
    }
    
    private void ValidateComponents()
    {
        if (turretTransform == null)
        {
            turretTransform = transform;
            Debug.LogWarning("TurretTransform is null! Use current transform");
        }

        if (firePoint == null)
        {
            Debug.LogError("FirePoint is null! Please assign fire point transform");
        }

        if (bulletPool == null)
        {
            Debug.LogError("BulletPool is null! Please assign bullet pool");
        }
    }
    
    public bool CanFire()
    {
        return Time.time >= lastFireTime + _weaponSettings.FireRate;
    }

    public void Fire()
    {
        if (bulletPool == null || firePoint == null) return;

        BulletController bulletController = bulletPool.GetBullet();
        if (bulletController == null) return;

        bulletController.transform.position = firePoint.position;
        bulletController.transform.rotation = firePoint.rotation;

        bulletController.Initialize(
            _weaponSettings.BulletSpeed,
            _weaponSettings.BulletDamage,
            _weaponSettings.BulletLifetime,
            bulletPool
        );

        lastFireTime = Time.time;
    }
}
