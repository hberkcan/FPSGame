using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalentButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    public int Index { get; set; }

    private Button button;

    private event Action<int> OnButtonPressed;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnButtonPressed(Index));
    }

    public void RegisterListener(Action<int> listener)
    {
        OnButtonPressed += listener;
    }

    public void RemoveListener(Action<int> listener) 
    {
        OnButtonPressed -= listener;
    }

    public void UpdateNameText(string name) 
    {
        nameText.text = name;
    }

    public void UpdateLevelText(int level) 
    {
        if (level == 0)
            return;

        levelText.text = level.ToString();
    }

    public void SetInteractable(bool interactable) 
    {
        button.interactable = interactable;
    }
}
