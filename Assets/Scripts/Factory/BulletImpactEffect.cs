using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletImpactEffect : MonoBehaviour, IPoolObject<BulletImpactEffectSettings>
{
    public BulletImpactEffectSettings Settings { get; set; }

    private void Awake()
    {
        var main = GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    public void Initialize(Vector3 position)
    {
        transform.position = position;
    }

    public void ReturnToPool()
    {
        Factory.Instance.BulletImpactEffectFactory.ReturnToPool(this);
    }

    public void SetSettings(PoolSettings settings)
    {
        Settings = (BulletImpactEffectSettings)settings;
    }

    void OnParticleSystemStopped()
    {
        ReturnToPool();
    }
}
