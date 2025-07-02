using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private WeaponSettings weaponSettings;

    private readonly Queue<BulletController> _bulletPool = new Queue<BulletController>();
    private readonly List<BulletController> _activeBullets = new List<BulletController>();

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        if (weaponSettings == null || weaponSettings.BulletPrefab == null)
        {
            Debug.LogError("WeaponSettings or bulletPrefab is null!");
            return;
        }

        GameObject bulletContainer = new GameObject("Bullet Container");
        bulletContainer.transform.SetParent(transform);

        for (int i = 0; i < weaponSettings.PoolSize; i++)
        {
            GameObject bulletObj = Instantiate(weaponSettings.BulletPrefab, bulletContainer.transform);
            BulletController bulletController = bulletObj.GetComponent<BulletController>();

            if (bulletController == null)
            {
                bulletController = bulletObj.AddComponent<BulletController>();
            }

            if (bulletObj.GetComponent<Collider>() == null)
            {
                SphereCollider collider = bulletObj.AddComponent<SphereCollider>();
                collider.isTrigger = true;
                collider.radius = 0.1f;
            }

            bulletObj.SetActive(false);
            _bulletPool.Enqueue(bulletController);
        }
    }

    public BulletController GetBullet()
    {
        if (_bulletPool.Count > 0)
        {
            BulletController bulletController = _bulletPool.Dequeue();
            _activeBullets.Add(bulletController);
            bulletController.gameObject.SetActive(true);
            return bulletController;
        }

        Debug.LogWarning("No bullets available in pool!");
        return null;
    }

    public void ReturnBullet(BulletController bulletController)
    {
        if (bulletController == null) return;

        bulletController.ResetBullet();

        _activeBullets.Remove(bulletController);

        bulletController.gameObject.SetActive(false);
        _bulletPool.Enqueue(bulletController);
    }

    private void OnDestroy()
    {
        // Очищуємо при знищенні
        _bulletPool.Clear();
        _activeBullets.Clear();
    }
}