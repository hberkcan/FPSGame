using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarPresenter : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    private static readonly string health = "_Health";

    public void UpdateHealthBar(float normalizedValue) 
    {
        rend.material.SetFloat(health, normalizedValue);
    }
}
