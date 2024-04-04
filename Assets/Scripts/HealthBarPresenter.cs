using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarPresenter : MonoBehaviour
{
    [SerializeField] private Image image;

    public void UpdateHealthBar(float normalizedValue) 
    {
        image.fillAmount = normalizedValue;
    }
}
