using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraShake : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin channelPerlin;
    private void Awake()
    {
        channelPerlin = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnEnable()
    {
        PlayerInputController.IsShooting += PlayerController_IsShooting;
    }

    private void PlayerController_IsShooting(bool isShooting)
    {
        if (isShooting) { ShakeCamera(0.1f, 0.2f); } else { ShakeCamera(0, 0.2f); }
    }
    

    private void ShakeCamera(float amplitude, float frequency)
    {
        channelPerlin.m_AmplitudeGain = amplitude;
        channelPerlin.m_FrequencyGain = frequency;
    }
}
