using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEditor.PackageManager.Requests;
using System;
using System.Linq;
using static CameraShake;

public class CameraShake : Singleton<CameraShake>
{
    public enum ShakeType { Recoil, Damage}

    private CinemachineVirtualCamera cam;
    private CinemachineBasicMultiChannelPerlin channelPerlin;

    private Dictionary<ShakeType, ShakeRequest> shakeRequests = new();

    private float lastShakeAmplitude;
    private float shakeDampingTime = 0;
    private const float shakeDampingTotalTime = 0.1f;

    protected override void Awake()
    {
        base.Awake();

        cam = GetComponent<CinemachineVirtualCamera>();
        channelPerlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (shakeRequests.Count == 0) 
        {
            if(channelPerlin.m_AmplitudeGain != 0) 
            {
                shakeDampingTime += Time.deltaTime;
                channelPerlin.m_AmplitudeGain = Mathf.Lerp(lastShakeAmplitude, 0, Mathf.Clamp01(shakeDampingTime / shakeDampingTotalTime));
            }
            else 
            {
                shakeDampingTime = 0;
            }

            return;
        }

        foreach (var shakeRequest in shakeRequests.Values)
        {
            shakeRequest.Update(Time.deltaTime);
        }

        var strongestShake = shakeRequests.Values.Max(i => i.Intensity);
        lastShakeAmplitude = strongestShake;
        channelPerlin.m_AmplitudeGain = lastShakeAmplitude;

        foreach (var shakeRequest in shakeRequests.Where(r => r.Value.MarkedForRemoval == true).ToList())
        {
            shakeRequests[shakeRequest.Key].Dispose();
        }
    }

    public void RequestShake(ShakeType type ,float intensity, float time) 
    {
        if (!shakeRequests.ContainsKey(type)) 
        {
            var request = new ShakeRequest(intensity, time);
            shakeRequests.Add(type, request);
            request.OnDispose += _ => {
                shakeRequests.Remove(type);
            };
        }
        else 
        {
            var request = shakeRequests[type];
            request.Intensity = intensity;
            request.ShakeTimer.Reset(time);
        }
    }
}

public class ShakeRequest : IDisposable
{
    public float Intensity;
    public CountdownTimer ShakeTimer;

    public event Action<ShakeRequest> OnDispose;
    public bool MarkedForRemoval {  get; set; }

    public ShakeRequest(float intensity, float time) 
    {
        if (time < 0) return;

        this.Intensity = intensity;
        ShakeTimer = new CountdownTimer(time);
        ShakeTimer.OnTimerStop += () => MarkedForRemoval = true;
        ShakeTimer.Start();
    }

    public void Dispose()
    {
        OnDispose?.Invoke(this);
    }

    public void Update(float deltaTime) => ShakeTimer?.Tick(deltaTime);
}
