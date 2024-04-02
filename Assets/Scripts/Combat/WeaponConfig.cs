using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    public ProjectileSettings ProjectileSettings;
    public float BulletSpawnForce = 50;
    public float FireRate = 15;
    public int AmmoCapacity = 120;
    public int Damage = 10;
    public bool CanPierce = false;
    public LayerMask HitMask;
    public GameObject ImpactEffect;
}
