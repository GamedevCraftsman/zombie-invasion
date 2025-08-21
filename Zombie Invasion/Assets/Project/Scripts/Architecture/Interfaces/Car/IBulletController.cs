using UnityEngine;

public interface IBulletController
{
    void Initialize(float bulletSpeed, int bulletDamage, float bulletLifetime, BulletPool pool);
    void ChangeBulletState(bool state);
    void SetFirePoint(Transform firePoint);
    void ResetBullet();
}
