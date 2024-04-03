using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private TalentTreeView talentTreeView;

    private void OnEnable()
    {
        InputManager.MenuInputs.IsTalentTreeViewOpen.AddListener(InputManager_TalentTreeMenuStatusChanged);
    }

    private void InputManager_TalentTreeMenuStatusChanged(bool value)
    {
        talentTreeView.gameObject.SetActive(value);
    }
}
