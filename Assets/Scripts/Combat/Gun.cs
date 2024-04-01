using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Gun : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponConfig config;

    [SerializeField] private Transform fpsCam;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private ParticleSystem muzzleFlash;
    
    private float nextTimeToFire = 0;

    private int currentAmmo;

    public void Use()
    {
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / config.FireRate;

            muzzleFlash.Play();

            RaycastHit hit;
            Vector3 shootDirection = shootPoint.forward;

            if (Physics.Raycast(fpsCam.position, fpsCam.forward, out hit, float.MaxValue, config.HitMask)) 
            {
                Vector3 directionToHit = (hit.point - shootPoint.position).normalized;
                shootDirection = directionToHit;
            }

            transform.forward = shootDirection;

            Projectile projectile = Factory.ProjectileFactory.Spawn(config.ProjectileSettings);
            projectile.Initialize(shootPoint.position);
            projectile.OnCollision = HandleProjectileCollision;
            projectile.Launch(shootDirection * config.BulletSpawnForce);
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        currentAmmo = Mathf.Min(currentAmmo, config.MaxAmmo);
    }

    private void HandleProjectileCollision(Projectile projectile, Collision collision)
    {
        ContactPoint contactPoint = collision.GetContact(0);

        if (contactPoint.otherCollider.TryGetComponent(out IDamagable damagable))
        {
            projectile.EntitiesPenetrated++;
            damagable.TakeDamage(config.Damage);
        }
        else 
        {
            projectile.ReturnToPool();
        }

        HandleProjectileImpact(contactPoint.point, contactPoint.normal);
    }

    private void HandleProjectileImpact(Vector3 hitLocation, Vector3 hitNormal)
    {
        Instantiate(config.ImpactEffect, hitLocation, Quaternion.LookRotation(hitNormal));
    }
}
