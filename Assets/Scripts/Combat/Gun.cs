using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float shootTime;

    private void Update()
    {
        if (shootTime > 0)
        {
            float recoverySpeed = 0.35f;
            shootTime -= Time.deltaTime * recoverySpeed;
        }
    }
    public void Use()
    {
        if (shootTime < config.MaxSpreadTime)
        {
            shootTime += Time.deltaTime;
        }

        if (Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f / config.FireRate;

            muzzleFlash.Play();

            if (useFPSCam) 
            {
                Shoot(fpsCam);
            }
            else 
            {
                Shoot(shootPoint);
            }

            if (!infiniteAmmo)
                currentAmmo--;

            OnShoot?.Invoke();
        }
    }

    private void Shoot(Transform shootOrigin) 
    {
        float maxTraceRange = 50f;
        var trace = CreateBulletTrace(shootPoint.position);

        Ray ray = new(shootOrigin.position, shootOrigin.forward);
        RaycastHit hit;

        if (RaycastSegment(ray, out hit))
        {
            trace.transform.position = hit.point;
        }
        else
        {
            trace.transform.position = shootOrigin.position + shootOrigin.forward * maxTraceRange;
        }
    }

    private bool RaycastSegment(Ray ray, out RaycastHit hit) 
    {
        if (Physics.Raycast(ray, out hit, float.MaxValue, config.HitMask))
        {
            HandleProjectileCollision(ray, hit);
            return true;
        }

        return false;
    }

    private BulletTrace CreateBulletTrace(Vector3 origin) 
    {
        var trace = Factory.Instance.BulletTraceFactory.Spawn(config.BulletTraceSettings);
        trace.Initialize(origin);
        return trace;
    }

    private void HandleProjectileCollision(Ray ray, RaycastHit hit)
    {
        if (hit.transform.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(damage);

            if (CanPierce)
            {
                float pierceOffset = 1f;

                Ray newRay = new(hit.point + ray.direction.normalized * pierceOffset, ray.direction);
                RaycastSegment(newRay, out hit);
            }
        }

        HandleProjectileImpact(hit.point, hit.normal);
    }

    private void HandleProjectileImpact(Vector3 hitLocation, Vector3 hitNormal)
    {
        var impact = Factory.Instance.BulletImpactEffectFactory.Spawn(config.ImpactEffectSettings);
        impact.Initialize(hitLocation);
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        currentAmmo = Mathf.Min(currentAmmo, ammoCapacity);
    }

    public void UpgradeDamage(int amount) => damage += amount;
    public void UpgradeAmmoCapacity(int amount) => ammoCapacity += amount;
    public void SetDamage(int value) => damage = value;

    public Vector3 Spread => config.GetSpread(shootTime);
}
