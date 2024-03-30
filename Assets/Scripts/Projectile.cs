using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour, IPoolObject<ProjectileSettings>
{
    private Rigidbody rb;
    private float duration;

    public ProjectileSettings Settings { get; set; }
    public IProjectileBehaviour Behaviour { get; set; }
    public bool IsPooled { get; set; } = true;

    TrailRenderer trailRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        duration -= Time.deltaTime;

        if (duration <= 0)
        {
            if (!IsPooled) 
            {
                Destroy(gameObject);
                return;
            }

            ReturnToPool();
        }
    }

    private void FixedUpdate()
    {
        if (Behaviour != null)
            Behaviour.MoveProjectile(rb);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(Settings.Damage);

            if (!IsPooled)
                Destroy(gameObject);

            ReturnToPool();
        }
    }

    public void Initialize()
    {
        if (!IsPooled) 
        {
            duration = 2f;
            return;
        }

        duration = Settings.Duration;
        trailRenderer.Clear();
    }

    public void SetSettings(PoolSettings settings) => Settings = (ProjectileSettings)settings;

    public void ReturnToPool()
    {
        Factory.ProjectileFactory.ReturnToPool(this);
    }
}
