using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PoolObjectFactory<T> where T : MonoBehaviour, IPoolObject
{
    readonly Dictionary<string, IObjectPool<T>> pools = new();

    protected IObjectPool<T> GetPoolFor(PoolSettings<T> settings)
    {
        if (pools.TryGetValue(settings.ID, out IObjectPool<T> pool)) return pool;

        pool = new ObjectPool<T>(
            settings.Create,
            settings.OnGet,
            settings.OnRelease,
            settings.OnDestroyPoolObject,
            settings.collectionCheck,
            settings.defaultCapacity,
            settings.maxPoolSize
        );

        pools.Add(settings.ID, pool);
        return pool;
    }

    public abstract T Spawn(PoolSettings<T> settings);
    public abstract void ReturnToPool(T poolObject);
}

public class ProjectileFactory : PoolObjectFactory<Projectile>
{
    public override Projectile Spawn(PoolSettings<Projectile> settings) => GetPoolFor(settings)?.Get();
    public override void ReturnToPool(Projectile poolObject) => GetPoolFor(poolObject.Settings)?.Release(poolObject);
}
