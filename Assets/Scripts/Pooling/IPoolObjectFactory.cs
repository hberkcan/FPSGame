using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObjectFactory<T,U> where T : IPoolObject where U : PoolSettings
{
    public T Spawn(U settings);
    public void ReturnToPool(T poolObject);
}
