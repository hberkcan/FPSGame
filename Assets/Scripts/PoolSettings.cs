using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolSettings : ScriptableObject
{
    public string ID;
    public GameObject prefab;
    public bool collectionCheck = true;
    public int defaultCapacity = 10;
    public int maxPoolSize = 100;
}

public abstract class PoolSettings<T> : PoolSettings where T : MonoBehaviour, IPoolObject
{
    public virtual T Create()
    {
        var go = Instantiate(prefab);
        go.SetActive(false);
        go.name = prefab.name;

        var poolObject = go.GetOrAdd<T>();
        poolObject.SetSettings(this);

        return poolObject;
    }

    public virtual void OnGet(T go) => go.gameObject.SetActive(true);

    public virtual void OnRelease(T go) => go.gameObject.SetActive(false);

    public virtual void OnDestroyPoolObject(T go) => Destroy(go.gameObject);
}
