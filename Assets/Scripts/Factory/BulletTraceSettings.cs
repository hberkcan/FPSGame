using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PoolObject/Bullet Trace Settings")]
public class BulletTraceSettings : PoolSettings<BulletTrace>
{
    public override void OnGet(BulletTrace go)
    {
        // set active in initialize method to avoid uncleared trail
    }
}
