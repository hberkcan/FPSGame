using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PoolObject/Projectile Settings")]
public class ProjectileSettings : PoolSettings<Projectile>
{
    public float Duration = 2f;
}
