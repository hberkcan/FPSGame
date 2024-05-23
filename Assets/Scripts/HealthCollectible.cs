using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HealthCollectible : MonoBehaviour, ICollectable
{
    [SerializeField] private int amount = 10;
    [SerializeField] private TextMeshPro text;
    public Action<CollectibleType> OnDestroy { get; set; }
    [field: SerializeField] public CollectibleType CollectibleType { get; set; }

    public void OnCollect(PlayerController playerController)
    {
        if (playerController.CurrentHealth == playerController.MaxHealth)
            return;

        playerController.AddHealth(amount);
        OnDestroy?.Invoke(CollectibleType);
        Destroy(gameObject);
    }

    private void Update()
    {
        text.text = $"{amount}";
    }
}
