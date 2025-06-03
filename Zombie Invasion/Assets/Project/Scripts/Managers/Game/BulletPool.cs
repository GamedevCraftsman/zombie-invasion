using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private WeaponSettings weaponSettings;

    private Queue<Bullet> bulletPool = new Queue<Bullet>();
    private List<Bullet> activeBullets = new List<Bullet>();

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
            Bullet bullet = bulletObj.GetComponent<Bullet>();

            if (bullet == null)
            {
                bullet = bulletObj.AddComponent<Bullet>();
            }

            if (bulletObj.GetComponent<Collider>() == null)
            {
                SphereCollider collider = bulletObj.AddComponent<SphereCollider>();
                collider.isTrigger = true;
                collider.radius = 0.1f;
            }

            bulletObj.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    public Bullet GetBullet()
    {
        if (bulletPool.Count > 0)
        {
            Bullet bullet = bulletPool.Dequeue();
            activeBullets.Add(bullet);
            bullet.gameObject.SetActive(true);
            return bullet;
        }

        Debug.LogWarning("No bullets available in pool!");
        return null;
    }

    public void ReturnBullet(Bullet bullet)
    {
        if (bullet == null) return;

        bullet.ResetBullet();

        activeBullets.Remove(bullet);

        bullet.gameObject.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    private void OnDestroy()
    {
        // Очищуємо при знищенні
        bulletPool.Clear();
        activeBullets.Clear();
    }
}