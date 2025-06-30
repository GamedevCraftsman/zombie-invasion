using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float _speed;
    private int _damage;
    private float _lifetime;
    private BulletPool _bulletPool;
    private Vector3 _direction;
    private bool _isActive;
    
    public void Initialize(float bulletSpeed, int bulletDamage, float bulletLifetime, BulletPool pool)
    {
        _speed = bulletSpeed;
        _damage = bulletDamage;
        _lifetime = bulletLifetime;
        _bulletPool = pool;
        _direction = transform.forward;
        _isActive = true;
        
        StartCoroutine(LifetimeTimer());
    }
    
    private void FixedUpdate()
    {
        if (!_isActive) return;
        
        transform.position += _direction * (_speed * Time.fixedDeltaTime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;
        
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damage);
            
            ReturnToPool();
        }
    }
    
    private IEnumerator LifetimeTimer()
    {
        yield return new WaitForSeconds(_lifetime);
        ReturnToPool();
    }
    
    private void ReturnToPool()
    {
        if (!_isActive) return;
        
        _isActive = false;
        
        StopAllCoroutines();
        
        if (_bulletPool != null)
        {
            _bulletPool.ReturnBullet(this);
        }
    }
    
    public void ResetBullet()
    {
        StopAllCoroutines();
        _isActive = false;
        _direction = Vector3.forward;
    }
}