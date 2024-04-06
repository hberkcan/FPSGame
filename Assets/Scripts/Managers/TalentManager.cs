using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentManager : MonoBehaviour
{
    [SerializeField] private TalentTreeView talentTreeView;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private List<TalentData> talentDatas;

    public readonly Dictionary<TalentType, Talent> UnlockedTalents = new();

    private int talentPoints = 0;

    private void OnEnable()
    {
        playerController.OnGetKill += PlayerController_OnGetKill;
    }

    private void OnDisable()
    {
        playerController.OnGetKill -= PlayerController_OnGetKill;
    }

    private void PlayerController_OnGetKill()
    {
        talentPoints++;
        talentTreeView.UpdateTalentPointsText(talentPoints);
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < talentDatas.Count; i++)
        {
            TalentButton talentButton = talentTreeView.ButtonList[i];
            talentButton.Index = i;
            talentButton.RegisterListener(OnTalentButtonPressed);

            talentTreeView.UpdateTalentPointsText(talentPoints);
            talentTreeView.UpdateTalentButtonNameText(i, talentDatas[i].DisplayName);
            talentTreeView.UpdateTalentButtonLevelText(i, 0);
        }
    }

    private void OnTalentButtonPressed(int index)
    {
        TalentData talentData = talentDatas[index];

        if (!CanAffordTalent(talentData))
            return;

        if (!IsTalentUnlocked(talentData.Type)) 
        {
            Talent talent = UnlockTalent(talentData);
            int newLevel = ++talent.CurrentLevel;
            talentTreeView.UpdateTalentButtonLevelText(index, newLevel);
            SpendTalentPoints(talentData.Cost);
            UpgradeStat(talent);
        }
        else
        {
            Talent talent = UnlockedTalents[talentData.Type];

            if (talent.CurrentLevel < talentData.MaxLevel) 
            {
                int newLevel = ++talent.CurrentLevel;
                talentTreeView.UpdateTalentButtonLevelText(index, newLevel);
                SpendTalentPoints(talentData.Cost);
                UpgradeStat(talent);
            }
        }
    }

    public Talent UnlockTalent(TalentData talentData)
    {
        if (!IsTalentUnlocked(talentData.Type)) 
        {
            Talent talent = talentData.CreateTalent();
            UnlockedTalents.Add(talentData.Type, talent);
            return talent;
        }

        return null;
    }

    public bool IsTalentUnlocked(TalentType talentType)
    {
        return UnlockedTalents.ContainsKey(talentType);
    }

    public bool CanAffordTalent(TalentData data)
    {
        return talentPoints >= data.Cost;
    }

    private void SpendTalentPoints(int amount) 
    {
        talentPoints -= amount;
        talentTreeView.UpdateTalentPointsText(talentPoints);
    }

    private void UpgradeStat(Talent talent) 
    {
        switch (talent.Data.Type) 
        {
            case TalentType.Player_Speed:
                playerController.UpgradeMoveSpeed(talent.Data.IncrementDatas[talent.CurrentLevel - 1].Increment);
                break;
            case TalentType.Player_JumpHeight:
                playerController.UpgradeJumpHeight(talent.Data.IncrementDatas[talent.CurrentLevel - 1].Increment);
                break;
            case TalentType.Player_Health:
                playerController.UpgradeMaxHealth((int)talent.Data.IncrementDatas[talent.CurrentLevel - 1].Increment);
                break;
            case TalentType.Gun_Damage:
                playerController.UpgradeGunDamage((int)talent.Data.IncrementDatas[talent.CurrentLevel - 1].Increment);
                break;
            case TalentType.Gun_Ammo_Capacity:
                playerController.UpgradeGunAmmoCapacity((int)talent.Data.IncrementDatas[talent.CurrentLevel - 1].Increment);
                break;
            case TalentType.Gun_Pierce:
                playerController.UpgradeGunPierce(true);
                break;
        }
    }
}
