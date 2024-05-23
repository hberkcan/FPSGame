using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;

public class BulletTrace : MonoBehaviour, IPoolObject<BulletTraceSettings>
{
    private TrailRenderer trailRenderer;
    private float duration;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        duration -= Time.deltaTime;

        if (duration <= 0)
            ReturnToPool();
    }

    public BulletTraceSettings Settings { get; set; }

    public void Initialize(Vector3 position)
    {
        transform.position = position;
        trailRenderer.Clear();
        duration = trailRenderer.time;
        trailRenderer.AddPosition(position);
        gameObject.SetActive(true);
    }

    public void ReturnToPool()
    {
        Factory.Instance.BulletTraceFactory.ReturnToPool(this);
    }

    public void SetSettings(PoolSettings settings) => Settings = (BulletTraceSettings)settings;
}
