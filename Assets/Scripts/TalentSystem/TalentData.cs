using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TalentType
{
    Player_Speed,
    Player_JumpHeight,
    Player_Health,
    Gun_Damage,
    Gun_Ammo_Capacity,
    Gun_Pierce
}

[CreateAssetMenu(menuName = "Talent Data")]
public class TalentData : ScriptableObject
{
    public TalentType Type;
    public string DisplayName;
    public int Cost;
    public int MaxLevel;
    public List<TalentIncrementData> IncrementDatas;

    public Talent CreateTalent()
    {
        return new Talent(this);
    }
}

public class Talent 
{
    public TalentData Data;
    public int CurrentLevel;

    public Talent(TalentData data)
    {
        Data = data;
        CurrentLevel = 0;
    }
}

[Serializable]
public class TalentIncrementData 
{
    public int Level;
    public float Increment;
}
