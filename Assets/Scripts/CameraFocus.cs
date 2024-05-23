using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private InputReader input;

    [SerializeField] private float mainFOV = 40f;
    [SerializeField] private float focusFOV = 30f;

    private float duration = 0.1f;
    private float tempFOV;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        tempFOV = mainFOV;
        virtualCamera.m_Lens.FieldOfView = mainFOV;
    }

    private void OnEnable()
    {
        input.Focus += Focus;
    }

    private void OnDisable()
    {
        input.Focus -= Focus;
    }

    private void Focus(bool isFocusing)
    {
        if (isFocusing)
        {
            DOTween.To(() => tempFOV, x => tempFOV = x, focusFOV, duration)
                .OnUpdate(() => {
                    virtualCamera.m_Lens.FieldOfView = tempFOV;
                });
        }
        else
        {
            DOTween.To(() => tempFOV, x => tempFOV = x, mainFOV, duration)
                .OnUpdate(() => {
                    virtualCamera.m_Lens.FieldOfView = tempFOV;
                });
        }
    }
}
