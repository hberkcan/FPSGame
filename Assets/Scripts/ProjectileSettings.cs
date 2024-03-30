using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PoolObject/Projectile Settings")]
public class ProjectileSettings : PoolSettings<Projectile>
{
    public int Damage = 10;
    public float ProjectileSpeed = 10f;
    public float Duration = 2f;
}
