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

    public void UpdateHealth(float normalizedValue, int currentHealth, int maxHealth) 
    {
        healthBarFill.fillAmount = normalizedValue;
        healthText.text = $"{currentHealth}/{maxHealth}";
    }
    public void UpdateExp(float normalizedValue, int Level) 
    {
        expBarFill.fillAmount = normalizedValue;
        expText.text = $"{Level}";
    }
    public void UpdateAmmo(int currentAmmo, int maxAmmo) => ammoText.text = $"Ammo : {currentAmmo}/{maxAmmo}";
    public void UpdateKills(int value) => killsText.text = $"Kills : {value}";
}
