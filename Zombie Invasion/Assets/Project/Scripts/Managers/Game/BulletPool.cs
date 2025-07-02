using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BulletPool : MonoBehaviour
{
    [Inject] private WeaponSettings _weaponSettings;

    private readonly Queue<IBulletController> _bulletPool = new ();
    private readonly List<IBulletController> _activeBullets = new ();

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        if (_weaponSettings == null || _weaponSettings.BulletPrefab == null)
        {
            Debug.LogError("WeaponSettings or bulletPrefab is null!");
            return;
        }
        
        SpawnBullets();
    }

    private void SpawnBullets()
    {
        for (int i = 0; i < _weaponSettings.PoolSize; i++)
        {
            GameObject bulletObj = Instantiate(_weaponSettings.BulletPrefab, this.transform);
            IBulletController bulletController = bulletObj.GetComponent<IBulletController>();

            Validate(bulletController);

            bulletObj.SetActive(false);
            _bulletPool.Enqueue(bulletController);
        }
    }
    
    private void Validate(IBulletController bulletController)
    {
        if (bulletController == null)
        {
            Debug.LogError("BulletController is null!");
        }
    }
    
    public IBulletController GetBullet()
    {
        if (_bulletPool.Count > 0)
        {
            return Get();
        }

        Debug.LogWarning("No bullets available in pool!");
        return null;
    }

    private IBulletController Get()
    {
        IBulletController bulletController = _bulletPool.Dequeue();
        _activeBullets.Add(bulletController);
        bulletController.ChangeBulletState(true);
        return bulletController;
    }
    
    public void ReturnBullet(IBulletController bulletController)
    {
        if (bulletController == null) return;

        bulletController.ResetBullet();

        _activeBullets.Remove(bulletController);

        bulletController.ChangeBulletState(false);
        _bulletPool.Enqueue(bulletController);
    }

    private void OnDestroy()
    {
        _bulletPool.Clear();
        _activeBullets.Clear();
    }
}