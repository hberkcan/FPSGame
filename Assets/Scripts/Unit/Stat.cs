using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    private float value;
    private float modifiers;
    private bool isDirty = true;

    public float Value
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

    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
    }

    public void IncrementValue(float amount)
    {
        isDirty = true;
        modifiers += amount;
    }

    private float CalculateFinalValue()
    {
        float finalValue = baseValue;
        finalValue += modifiers;
        return finalValue;
    }
}
