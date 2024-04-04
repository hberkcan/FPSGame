using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectibleType 
{
    Ammo,
    Health
}
public interface ICollectable
{
    public void OnCollect(PlayerController playerController);
    public Action<CollectibleType> OnDestroy {  get; set; }
    public CollectibleType CollectibleType { get; set; }
}
