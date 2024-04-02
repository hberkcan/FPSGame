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

    private int damage;
    private int ammoCapacity;
    public bool CanPierce {  get; set; }

    private void Start()
    {
        damage = config.Damage;
        ammoCapacity = config.AmmoCapacity;
        CanPierce = config.CanPierce;
    }

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

    private void HandleProjectileCollision(Projectile projectile, Collision collision)
    {
        ContactPoint contactPoint = collision.GetContact(0);

        if (contactPoint.otherCollider.TryGetComponent(out IDamagable damagable))
        {
            if (!CanPierce) 
            {
                damagable.TakeDamage(damage);
                return;
            }

            projectile.EntitiesPenetrated++;
            damagable.TakeDamage(damage);
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

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        currentAmmo = Mathf.Min(currentAmmo, ammoCapacity);
    }

    public void UpgradeDamage(int value) => damage += value;
    public void UpgradeAmmoCapacity(int value) => ammoCapacity += value;
}
