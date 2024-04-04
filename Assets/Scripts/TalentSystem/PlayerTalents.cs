using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTalents : MonoBehaviour
{
    public readonly Dictionary<TalentType, Talent> UnlockedTalents = new();

    public void UnlockTalent(Talent talent) 
    {
        if (!IsTalentUnlocked(talent.Data.Type))
            UnlockedTalents.Add(talent.Data.Type, talent);
    }

    public bool IsTalentUnlocked(TalentType talentType) 
    {
        return UnlockedTalents.ContainsKey(talentType);
    }
}
