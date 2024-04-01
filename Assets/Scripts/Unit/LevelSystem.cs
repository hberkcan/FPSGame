using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelSystem
{
    public int CurrentLevel { get; private set; } = 1;
    public int CurrentExperience { get; private set; } = 0;
    public int XpRequiredForNextLevel => GetXPRequiredForNextLevel();
    public double XPPercentage => GetXPPercentage();

    private readonly int offset = 5;
    private readonly float slope = 5f;
    private readonly int levelCap = 10;

    public bool AddXP(int amount)
    {
        if (CurrentLevel == levelCap)
            return false;

        CurrentExperience += amount;

        int newLevel = Mathf.Min(Mathf.FloorToInt((CurrentExperience - offset) / slope) + 2, levelCap);
        bool leveledUp = newLevel != CurrentLevel;

        CurrentLevel = newLevel;
        return leveledUp;
    }

    public void SetLevel(int level)
    {
        CurrentExperience = 0;
        CurrentLevel = level;
        AddXP(XPForLevel(level));
    }

    private int XPForLevel(int level)
    {
        return Mathf.FloorToInt((Mathf.Min(level, levelCap) - 1) * slope + offset);
    }

    public int GetXPRequiredForNextLevel() 
    {
        if (CurrentLevel == levelCap)
            return int.MaxValue;

        return XPForLevel(CurrentLevel + 1) - CurrentExperience;
    }

    public void Reset()
    {
        CurrentLevel = 1;
        CurrentExperience = 0;
    }

    public double GetXPPercentage()
    {
        double x = CurrentExperience / slope;
        x -= Math.Floor(x);
        return Math.Round(x, 1) * 100;
    }
}
