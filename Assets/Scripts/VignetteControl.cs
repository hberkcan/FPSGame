using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VignetteControl : MonoBehaviour
{
    [SerializeField] private PostProcessVolume postProcessVolume;
    private Vignette vignette;

    private void Awake()
    {
        if (!postProcessVolume.profile.TryGetSettings(out vignette))
            Debug.LogError("Vignette Not Found");
    }

    private void OnEnable()
    {
        PlayerController.OnHealthChange += PlayerController_OnHealthChange;
    }

    private void OnDisable()
    {
        PlayerController.OnHealthChange -= PlayerController_OnHealthChange;
    }

    private void PlayerController_OnHealthChange(int change, float normalizedHealth)
    {
        SetIntensity((1 - normalizedHealth) * 0.5f);
    }

    private void SetIntensity(float intensity) => vignette.intensity.value = intensity;
}
