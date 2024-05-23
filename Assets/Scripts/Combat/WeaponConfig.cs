using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    public BulletTraceSettings BulletTraceSettings;
    public float FireRate = 15;
    public int AmmoCapacity = 120;
    public int Damage = 10;
    public bool CanPierce = false;
    public LayerMask HitMask;
    public BulletImpactEffectSettings ImpactEffectSettings;
    public Vector3 Spread = new(0.01f, 0.01f, 0.01f);
    public float MaxSpreadTime = 2f;

    public Vector3 GetSpread(float shootTime) 
    {
        Vector3 spread = Vector3.Lerp
            (Vector3.zero,
            new(
            Spread.x,
            Random.Range(-Spread.y, Spread.y),
            Random.Range(-Spread.z, Spread.z)
            ),
            Mathf.Clamp01(shootTime / MaxSpreadTime)
            );

        return spread;
    }
}
