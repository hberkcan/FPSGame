using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour, IPoolObject<ProjectileSettings>
{
    private Rigidbody rb;
    private float duration;
    public ProjectileSettings Settings { get; set; }

    private TrailRenderer trailRenderer;
    public int EntitiesPenetrated { get; set; } = 0;

    public Action<Projectile, Collision> OnCollision;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        duration -= Time.deltaTime;

        if (duration <= 0)
        {
            ReturnToPool();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision?.Invoke(this, collision);
    }

    public void Initialize(Vector3 position)
    {
        transform.position = position;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = position;
        rb.rotation = Quaternion.Euler(Vector2.zero);
        trailRenderer.Clear();
        EntitiesPenetrated = 0;

        gameObject.SetActive(true);
    }

    public void SetSettings(PoolSettings settings) => Settings = (ProjectileSettings)settings;

    public void ReturnToPool()
    {
        duration = Settings.Duration;
        OnCollision = null;
        Factory.Instance.ProjectileFactory.ReturnToPool(this);
    }

    public void Launch(Vector3 startForce)
    {
        rb.AddForce(startForce, ForceMode.Impulse);
    }
}
