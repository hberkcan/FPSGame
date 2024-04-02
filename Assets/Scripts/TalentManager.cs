using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentManager : MonoBehaviour
{
    [SerializeField] private TalentTreeView talentTreeView;
    [SerializeField] private PlayerTalents playerTalents;

    [SerializeField] private List<TalentData> talentDatas;

    private int talentPoints = 10;

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

        if (!playerTalents.IsTalentUnlocked(talentData.Type)) 
        {
            UnlockTalent(talentData);
            Talent talent = playerTalents.UnlockedTalents[talentData.Type];
            int newLevel = ++talent.CurrentLevel;
            talentTreeView.UpdateTalentButtonLevelText(index, newLevel);
            SpendTalentPoints(talentData.Cost);
        }
        else
        {
            Talent talent = playerTalents.UnlockedTalents[talentData.Type];

            if (talent.CurrentLevel < talentData.MaxLevel) 
            {
                int newLevel = ++talent.CurrentLevel;
                talentTreeView.UpdateTalentButtonLevelText(index, newLevel);
                SpendTalentPoints(talentData.Cost);
            }
        }
    } 

    public void UnlockTalent(TalentData talentData)
    {
        playerTalents.UnlockTalent(talentData.CreateTalent());
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

    private void UpgradeStat(TalentType type) 
    {
        switch (type) 
        {
            case TalentType.Player_Speed:
                //
                break;
            case TalentType.Player_JumpHeight:
                //
                break;
            case TalentType.Player_Health:
                //
                break;
            case TalentType.Gun_Damage:
                //
                break;
            case TalentType.Gun_Ammo_Capacity:
                //
            case TalentType.Gun_Pierce:
                //
                break;
        }
    }
}
