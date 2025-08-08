using System;
using Zenject;
using System.Threading.Tasks;
using UnityEngine;

public class FireController : BaseController, IFireController
{
    [Header("References")] 
    [SerializeField] private Transform turretTransform;
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletPool bulletPool;
    
    [Inject] private WeaponSettings _weaponSettings;
    
    private float _lastFireTime = 0f;
    
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
    
    private bool CanFire()
    {
        return Time.time >= _lastFireTime + _weaponSettings.FireRate;
    }

    public void Fire()
    {
        if (bulletPool == null || firePoint == null || !CanFire()) return;

        IBulletController bulletController = bulletPool.GetBullet();
        if (bulletController == null) return;
        
        bulletController.SetFirePoint(firePoint);
        fireParticles.Play();
        
        bulletController.Initialize(
            _weaponSettings.BulletSpeed,
            _weaponSettings.BulletDamage,
            _weaponSettings.BulletLifetime,
            bulletPool
        );

        _lastFireTime = Time.time;
    }
}
