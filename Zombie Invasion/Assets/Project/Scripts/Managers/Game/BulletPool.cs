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
        if (weaponSettings == null || weaponSettings.bulletPrefab == null)
        {
            Debug.LogError("WeaponSettings or bulletPrefab is null!");
            return;
        }
        
        // Створюємо батьківський об'єкт для куль
        GameObject bulletContainer = new GameObject("Bullet Container");
        bulletContainer.transform.SetParent(transform);
        
        // Створюємо кулі в пулі
        for (int i = 0; i < weaponSettings.poolSize; i++)
        {
            GameObject bulletObj = Instantiate(weaponSettings.bulletPrefab, bulletContainer.transform);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            
            if (bullet == null)
            {
                bullet = bulletObj.AddComponent<Bullet>();
            }
            
            // Додаємо Collider якщо його немає
            if (bulletObj.GetComponent<Collider>() == null)
            {
                SphereCollider collider = bulletObj.AddComponent<SphereCollider>();
                collider.isTrigger = true;
                collider.radius = 0.1f;
            }
            
            bulletObj.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
        
        Debug.Log($"Bullet pool initialized with {weaponSettings.poolSize} bullets");
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
        
        // Скидаємо стан кулі
        bullet.ResetBullet();
        
        // Прибираємо з активних
        activeBullets.Remove(bullet);
        
        // Деактивуємо та повертаємо в пул
        bullet.gameObject.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
    
    // Метод для отримання кількості активних куль (для дебагу)
    public int GetActiveBulletsCount()
    {
        return activeBullets.Count;
    }
    
    // Метод для отримання кількості доступних куль в пулі
    public int GetAvailableBulletsCount()
    {
        return bulletPool.Count;
    }
    
    private void OnDestroy()
    {
        // Очищуємо при знищенні
        bulletPool.Clear();
        activeBullets.Clear();
    }
}