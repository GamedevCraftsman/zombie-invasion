using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class TurretController : BaseController, ITurretController
{
    [Header("References")] 
    [SerializeField] private Transform turretTransform;

    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletPool bulletPool;

    // Injected dependencies
    [Inject] private WeaponSettings _weaponSettings;
    
    //State
    private bool controlEnabled = false;
    private float currentRotationAngle = 0f;

    // Shooting state
    private float lastFireTime = 0f;

    // Properties
    public bool IsControlEnabled => controlEnabled;
    public float CurrentRotationAngle => currentRotationAngle;

    // Events
    //public event Action<float> OnRotationChanged;
    public event Action OnGamePlaying;
    

    protected override Task Initialize()
    {
        try
        {
            ValidateComponents();
            ResetRotation();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
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

        if (_weaponSettings == null)
        {
            Debug.LogError("WeaponSettings is null!");
        }
    }
    
    private void Update()
    {
        if (!controlEnabled || _weaponSettings == null) return;

        OnGamePlaying?.Invoke();
        //HandleInput();
        //HandleShooting();
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

   
    public void SetRotation(float angle)
    {
        currentRotationAngle = angle;

        turretTransform.localRotation = Quaternion.Euler(0f, currentRotationAngle, 0f);
        
        //OnRotationChanged?.Invoke(currentRotationAngle);
        //EventBus.Fire(new TurretRotationEvent(currentRotationAngle, _weaponSettings.MaxRotationAngle));
    }

    public void EnableControl()
    {
        controlEnabled = true;
    }

    public void DisableControl()
    {
        controlEnabled = false;
    }

    public void ResetRotation()
    {
        SetRotation(0f);
    }

    private void OnDestroy()
    {
        OnGamePlaying = null;
        //OnRotationChanged = null;
    }

    #region Debud Info

    private void OnDrawGizmosSelected()
    {
        if (_weaponSettings == null) return;

        Vector3 center = transform.position;
        Vector3 forward = transform.forward;

        Vector3 leftBound = Quaternion.Euler(0, -_weaponSettings.MaxRotationAngle, 0) * forward;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(center, leftBound * 3f);

        Vector3 rightBound = Quaternion.Euler(0, _weaponSettings.MaxRotationAngle, 0) * forward;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(center, rightBound * 3f);

        Vector3 currentDirection = Quaternion.Euler(0, currentRotationAngle, 0) * forward;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(center, currentDirection * 4f);

        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
        }
    }

    #endregion
}