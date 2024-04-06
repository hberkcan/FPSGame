using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class Gun : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponConfig config;

    [SerializeField] private Transform fpsCam;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private ParticleSystem muzzleFlash;
    
    private float nextTimeToFire = 0;

    private int currentAmmo;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => config.AmmoCapacity;

    private int damage;
    private int ammoCapacity;
    public bool CanPierce {  get; set; }

    [SerializeField] private bool useFPSCam = false;
    [SerializeField] private bool infiniteAmmo = false;
    public event Action OnShoot;

    private void Awake()
    {
        damage = config.Damage;
        ammoCapacity = config.AmmoCapacity;
        currentAmmo = ammoCapacity;
        CanPierce = config.CanPierce;
    }

    public void Use()
    {
        if (Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f / config.FireRate;

            muzzleFlash.Play();

            RaycastHit hit;
            Vector3 shootDirection = shootPoint.forward;

            if(useFPSCam) 
            {
                if (Physics.Raycast(fpsCam.position, fpsCam.forward, out hit, float.MaxValue, config.HitMask))
                {
                    Vector3 directionToHit = (hit.point - shootPoint.position).normalized;
                    shootDirection = directionToHit;
                }
            }

            transform.forward = Vector3.MoveTowards(transform.forward, shootDirection, 0.1f * Time.deltaTime);

            LaunchNewProjectile(shootPoint.position, shootDirection);

            currentAmmo--;
            OnShoot?.Invoke();

            if (infiniteAmmo)
                currentAmmo++;
        }
    }

    private void LaunchNewProjectile(Vector3 origin, Vector3 direction)
    {
        Projectile projectile = Factory.Instance.ProjectileFactory.Spawn(config.ProjectileSettings);
        projectile.Initialize(origin);
        projectile.OnCollision = HandleProjectileCollision;
        projectile.Launch(direction * config.BulletSpawnForce);
    }

    private void HandleProjectileCollision(Projectile projectile, Collision collision)
    {
        ContactPoint contactPoint = collision.GetContact(0);

        if (contactPoint.otherCollider.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(damage);
            projectile.ReturnToPool();

            if (CanPierce) 
            {
                projectile.EntitiesPenetrated++;
                Vector3 direction = -collision.impulse.normalized;
                float penetrationOffset = 3f;
                Vector3 newOrigin = contactPoint.point + direction * penetrationOffset;
                LaunchNewProjectile(newOrigin, direction);
            }
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

    public void UpgradeDamage(int amount) => damage += amount;
    public void UpgradeAmmoCapacity(int amount) => ammoCapacity += amount;
    public void SetDamage(int value) => damage = value;
}
