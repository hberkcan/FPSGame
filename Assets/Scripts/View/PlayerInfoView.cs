using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerInfoView : View
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image expBarFill;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI killsText;

    public void UpdateHealth(float normalizedValue, int currentHealth) 
    {
        healthBarFill.fillAmount = normalizedValue;
        healthText.text = $"HP : {currentHealth}";
    }
    public void UpdateExp(float normalizedValue, int Level) 
    {
        expBarFill.fillAmount = normalizedValue;
        expText.text = $"Level : {Level}";
    }
    public void UpdateAmmo(int value) => ammoText.text = $"Ammo : {value}";
    public void UpdateKills(int value) => killsText.text = $"Kills : {value}";
}
