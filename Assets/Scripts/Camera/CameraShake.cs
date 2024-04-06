using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraShake : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin channelPerlin;
    private float shakeAmplitude = 0.1f;
    private float shakeFrequency = 0.2f;

    private void Awake()
    {
        channelPerlin = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnEnable()
    {
        InputManager.PlayerInputs.IsFiring.AddListener(PlayerInputs_IsFiring);
    }

    private void OnDisable()
    {
        InputManager.PlayerInputs.IsFiring.RemoveListener(PlayerInputs_IsFiring);
    }

    private void PlayerInputs_IsFiring(bool isShooting)
    {
        if (isShooting) { ShakeCamera(shakeAmplitude, shakeFrequency); } else { StopCameraShake(); }
    }
    

    private void ShakeCamera(float amplitude, float frequency)
    {
        channelPerlin.m_AmplitudeGain = amplitude;
        channelPerlin.m_FrequencyGain = frequency;
    }

    private void StopCameraShake() => ShakeCamera(0, 0f);
}
