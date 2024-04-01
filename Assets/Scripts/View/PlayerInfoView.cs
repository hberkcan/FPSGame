using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoView : View
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image expBarFill;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI killsText;

    public void UpdateHealth(float normalizedValue) => healthBarFill.fillAmount = normalizedValue;
    public void UpdateExp(float normalizedValue) => expBarFill.fillAmount = normalizedValue;
    public void UpdateAmmo(int value) => ammoText.text = $"Ammo : {value}";
    public void UpdateKills(int value) => killsText.text = $"Kills : {value}";
}
