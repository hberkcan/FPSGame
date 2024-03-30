using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Gun : MonoBehaviour, IWeapon
{
    [SerializeField] private Transform fpsCam;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;

    [SerializeField] private ProjectileSettings projectileSettings;

    [SerializeField] private float fireRate = 15;
    [SerializeField] private float range = 20f;

    private float nextTimeToFire = 0;
    
    public void Use()
    {
        if (Time.time >= nextTimeToFire)
        {
            //Projectile projectile = Factory.ProjectileFactory.Spawn(projectileSettings);
            //projectile.transform.position = shootPoint.position;
            //projectile.Initialize();
            //projectile.IsPooled = true;
            //projectile.Behaviour = new LinearProjectileBehaviour(transform.forward, projectileSettings.ProjectileSpeed);

            nextTimeToFire = Time.time + 1f / fireRate;

            muzzleFlash.Play();

            RaycastHit hit;
            if (Physics.Raycast(fpsCam.position, fpsCam.forward, out hit, range)) 
            {
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
}
