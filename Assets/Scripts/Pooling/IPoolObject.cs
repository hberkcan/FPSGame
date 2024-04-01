using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    public void Initialize(Vector3 position);
    public void SetSettings(PoolSettings settings);
    public void ReturnToPool();
}

public interface IPoolObject<T> : IPoolObject where T : PoolSettings 
{
    T Settings { get; set; }
}
