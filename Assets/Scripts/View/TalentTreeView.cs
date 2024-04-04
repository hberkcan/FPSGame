using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalentTreeView : View
{
    public List<TalentButton> ButtonList = new List<TalentButton>();
    [SerializeField] private TextMeshProUGUI talentPointsText;

    public void UpdateTalentButtonNameText(int buttonIndex, string text) => ButtonList[buttonIndex].UpdateNameText(text);
    public void UpdateTalentButtonLevelText(int buttonIndex, int level) => ButtonList[buttonIndex].UpdateLevelText(level);
    public void SetTalentButtonInteractable(int buttonIndex, bool interactable) => ButtonList[buttonIndex].SetInteractable(interactable);
    public void UpdateTalentPointsText(int talentPoints) => talentPointsText.text = $"Talent Points : {talentPoints}";
}
