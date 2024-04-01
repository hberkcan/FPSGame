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

    private void OnDisable()
    {
        trailRenderer.Clear();
    }

    private void Update()
    {
        duration -= Time.deltaTime;

        if (duration <= 0)
        {
            ReturnToPool();
        }

        //if(time < 1)
        //{
        //    transform.position = Vector3.Lerp(startPosition, destination, time);
        //    time += Time.deltaTime / 0.1f;
        //}
        //else 
        //{
        //    transform.position = destination;
        //    Instantiate(hitEffect, transform.position, Quaternion.identity);
        //    ReturnToPool();
        //    Debug.Log("HIT");
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision?.Invoke(this, collision);
    }

    public void Initialize(Vector3 position)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        trailRenderer.enabled = false;
        rb.position = position;
        trailRenderer.enabled = true;
        duration = Settings.Duration;
        OnCollision = null;
        EntitiesPenetrated = 0;
    }

    public void SetSettings(PoolSettings settings) => Settings = (ProjectileSettings)settings;

    public void ReturnToPool()
    {
        duration = Settings.Duration;
        OnCollision = null;
        Factory.ProjectileFactory.ReturnToPool(this);
    }

    public void Launch(Vector3 startForce)
    {
        rb.AddForce(startForce, ForceMode.Impulse);
    }
}
