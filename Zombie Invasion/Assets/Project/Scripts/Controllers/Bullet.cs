using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private int damage;
    private float lifetime;
    private BulletPool bulletPool;
    private Vector3 direction;
    private bool isActive = false;
    
    public void Initialize(float bulletSpeed, int bulletDamage, float bulletLifetime, BulletPool pool)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        lifetime = bulletLifetime;
        bulletPool = pool;
        direction = transform.forward;
        isActive = true;
        
        StartCoroutine(LifetimeTimer());
    }
    
    private void Update()
    {
        if (!isActive) return;
        
        transform.position += direction * speed * Time.deltaTime;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            
            ReturnToPool();
        }
    }
    
    private IEnumerator LifetimeTimer()
    {
        yield return new WaitForSeconds(lifetime);
        ReturnToPool();
    }
    
    private void ReturnToPool()
    {
        if (!isActive) return;
        
        isActive = false;
        
        StopAllCoroutines();
        
        if (bulletPool != null)
        {
            bulletPool.ReturnBullet(this);
        }
    }
    
    public void ResetBullet()
    {
        StopAllCoroutines();
        isActive = false;
        direction = Vector3.forward;
    }
}