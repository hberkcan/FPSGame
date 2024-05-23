using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoCollectible : MonoBehaviour, ICollectable
{
    [SerializeField] private int amount = 10;
    [SerializeField] private TextMeshPro text;
    public Action<CollectibleType> OnDestroy { get; set; }
    [field : SerializeField] public CollectibleType CollectibleType { get; set; }

    public void OnCollect(PlayerController playerController)
    {
        int maxAmount = playerController.MaxAmmo - playerController.CurrentAmmo;

        if(maxAmount >= amount)
        {
            playerController.AddAmmo(amount);
            OnDestroy?.Invoke(CollectibleType);
            Destroy(gameObject);
        }
        else 
        {
            int newAmount = amount - maxAmount;
            playerController.AddAmmo(newAmount);
            amount = newAmount;
        }
    }

    private void Update()
    {
        text.text = $"{amount}";
    }
}
