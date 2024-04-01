using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public void Use();
    public void AddAmmo(int amount);
}
