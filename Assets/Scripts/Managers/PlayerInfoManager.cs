using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    [SerializeField] private PlayerInfoView playerInfoView;
    [SerializeField] private PlayerController playerController;

    private void OnEnable()
    {
        PlayerController.OnHealthChange += PlayerController_OnHealthChange;
        playerController.OnXPGain += PlayerController_OnXPGain;
        playerController.OnGetKill += PlayerController_OnGetKill;
        playerController.OnAmmoChange += PlayerController_OnAmmoChange;
    }

    private void OnDisable()
    {
        PlayerController.OnHealthChange -= PlayerController_OnHealthChange;
        playerController.OnXPGain -= PlayerController_OnXPGain;
        playerController.OnGetKill -= PlayerController_OnGetKill;
        playerController.OnAmmoChange -= PlayerController_OnAmmoChange;
    }

    private void PlayerController_OnAmmoChange()
    {
        playerInfoView.UpdateAmmo(playerController.CurrentAmmo, playerController.MaxAmmo);
    }

    private void PlayerController_OnGetKill()
    {
        playerInfoView.UpdateKills(playerController.KillScore);
    }

    private void PlayerController_OnXPGain()
    {
        playerInfoView.UpdateExp(playerController.XPPercentage, playerController.CurrentLevel);
    }

    private void PlayerController_OnHealthChange(int change, float normalizedHealth)
    {
        playerInfoView.UpdateHealth(playerController.GetHealthPercentage(), playerController.CurrentHealth, playerController.MaxHealth);
    }

    private void Start()
    {
        playerInfoView.UpdateHealth(playerController.GetHealthPercentage(), playerController.CurrentHealth, playerController.MaxHealth);
        playerInfoView.UpdateExp(playerController.XPPercentage, playerController.CurrentLevel);
        playerInfoView.UpdateAmmo(playerController.CurrentAmmo, playerController.MaxAmmo);
        playerInfoView.UpdateKills(playerController.KillScore);
    }
}
