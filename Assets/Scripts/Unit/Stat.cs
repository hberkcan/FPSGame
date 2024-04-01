using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private int baseValue;
    private int value;
    private int modifiers;
    private bool isDirty = true;

    public int Value
    {
        get 
        {
            if (isDirty)
            {
                value = CalculateFinalValue();
                isDirty = false;
            }

            return value;
        }
    }

    public Stat(int baseValue)
    {
        this.baseValue = baseValue;
    }

    public void IncrementValue(int amount)
    {
        isDirty = true;
        modifiers += amount;
    }

    private int CalculateFinalValue()
    {
        int finalValue = baseValue;
        finalValue += modifiers;
        return finalValue;
    }
}
