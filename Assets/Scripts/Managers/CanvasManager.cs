using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private TalentTreeView talentTreeView;
    [SerializeField] private GameOverView gameOverView;

    private void OnEnable()
    {
        InputManager.MenuInputs.IsTalentTreeViewOpen.AddListener(InputManager_TalentTreeMenuStatusChanged);
        PlayerController.OnPlayerDie += PlayerController_OnPlayerDie;
    }

    private void OnDisable()
    {
        InputManager.MenuInputs.IsTalentTreeViewOpen.RemoveListener(InputManager_TalentTreeMenuStatusChanged);
        PlayerController.OnPlayerDie -= PlayerController_OnPlayerDie;
    }

    private void PlayerController_OnPlayerDie()
    {
        gameOverView.Open();
    }

    private void InputManager_TalentTreeMenuStatusChanged(bool value)
    {
        talentTreeView.gameObject.SetActive(value);
    }
}
