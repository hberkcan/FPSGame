using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    [SerializeField] private PlayerInfoView playerInfoView;
    [SerializeField] private PlayerController playerController;

    private void OnEnable()
    {
        playerController.OnHealthChange += PlayerController_OnHealthChange;
        playerController.OnXPGain += PlayerController_OnXPGain;
        playerController.OnGetKill += PlayerController_OnGetKill;
        playerController.OnAmmoChange += PlayerController_OnAmmoChange;
    }

    private void OnDisable()
    {
        playerController.OnHealthChange -= PlayerController_OnHealthChange;
        playerController.OnXPGain -= PlayerController_OnXPGain;
        playerController.OnGetKill -= PlayerController_OnGetKill;
        playerController.OnAmmoChange -= PlayerController_OnAmmoChange;
    }

    private void PlayerController_OnAmmoChange()
    {
        playerInfoView.UpdateAmmo(playerController.CurrentAmmo);
    }

    private void PlayerController_OnGetKill()
    {
        playerInfoView.UpdateKills(playerController.KillScore);
    }

    private void PlayerController_OnXPGain()
    {
        playerInfoView.UpdateExp(playerController.XPPercentage, playerController.CurrentLevel);
    }

    private void PlayerController_OnHealthChange()
    {
        playerInfoView.UpdateHealth(playerController.GetHealthPercentage(), playerController.CurrentHealth);
    }

    private void Start()
    {
        playerInfoView.UpdateHealth(playerController.GetHealthPercentage(), playerController.CurrentHealth);
        playerInfoView.UpdateExp(playerController.XPPercentage, playerController.CurrentLevel);
        playerInfoView.UpdateAmmo(playerController.CurrentAmmo);
        playerInfoView.UpdateKills(playerController.KillScore);
    }
}
