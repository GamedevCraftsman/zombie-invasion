using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class BulletController : MonoBehaviour, IBulletController
{
    [SerializeField] private Rigidbody bulletRigidbody;

    private float _speed;
    private int _damage;
    private float _lifetime;

    private BulletPool _bulletPool;
    private Vector3 _direction;

    private bool _isActive;

    private Coroutine _coroutine;
    WaitForSeconds _wait;

    public void Initialize(float bulletSpeed, int bulletDamage, float bulletLifetime, BulletPool pool)
    {
        _speed = bulletSpeed;
        _damage = bulletDamage;
        _lifetime = bulletLifetime;
        _bulletPool = pool;
        _direction = transform.forward;
        _isActive = true;

        //Coroutine properties
        _wait = new WaitForSeconds(_lifetime);
        _coroutine = StartCoroutine(LifetimeTimer());
    }

    public void ChangeBulletState(bool state)
    {
        gameObject.SetActive(state);
    }

    public void SetFirePoint(Transform firePoint)
    {
        transform.position = firePoint.position;
        transform.rotation = firePoint.rotation;
    }

    private void FixedUpdate()
    {
        if (!_isActive) return;

        Move();
    }

    private void Move()
    {
        Vector3 newPos = bulletRigidbody.position + _direction * (_speed * Time.fixedDeltaTime);
        bulletRigidbody.MovePosition(newPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;

        EnemyController enemy = other.GetComponentInParent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damage);

            ReturnToPool();
        }
    }

    private IEnumerator LifetimeTimer()
    {
        yield return _wait;
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (!_isActive) return;

        _isActive = false;

        StopCoroutine(_coroutine);

        if (_bulletPool != null)
        {
            _bulletPool.ReturnBullet(this);
        }
    }

    public void ResetBullet()
    {
        StopCoroutine(_coroutine);
        _isActive = false;
        _direction = Vector3.forward;
    }
}