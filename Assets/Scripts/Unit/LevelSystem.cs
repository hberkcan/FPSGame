using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelSystem
{
    [SerializeField] private AnimationCurve levelCurve;
    [field: SerializeField] public int CurrentLevel { get; private set; } = 0;
    [field: SerializeField] public int CurrentExperience { get; private set; } = 0;
    public int XpRequiredForNextLevel => GetXPRequiredForNextLevel();
    public float XPPercentage => GetXPPercentage();
    
    [SerializeField] private int levelCap = 20;
    private const int levelCurveMultiplier = 10;

    public bool AddXP(int amount)
    {
        if (CurrentLevel == levelCap)
            return false;

        int previousLevel = CurrentLevel;
        CurrentExperience += amount;

        while(CurrentExperience >= GetXPRequiredForNextLevel()) 
        {
            CurrentLevel++;
        }

        return CurrentLevel > previousLevel;
    }

    public void SetLevel(int level)
    {
        CurrentExperience = 0;
        CurrentLevel = level;
        AddXP(XPForLevel(level));
    }

    public int XPForLevel(int level)
    {
        return (int)(levelCurve.Evaluate(level) * levelCurveMultiplier);
    }

    public int GetXPRequiredForNextLevel() 
    {
        if (CurrentLevel == levelCap)
            return int.MaxValue;

        return XPForLevel(CurrentLevel + 1);
    }

    public void Reset()
    {
        CurrentLevel = 0;
        CurrentExperience = 0;
    }

    public float GetXPPercentage()
    {
        return (float)CurrentExperience / XPForLevel(CurrentLevel + 1);
    }
}
