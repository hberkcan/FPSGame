using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private TalentTreeView talentTreeView;
    [SerializeField] private GameOverView gameOverView;
    [SerializeField] private GameObject crossHair;

    private void OnEnable()
    {
        input.TabButtonAction += TabButtonAction;
        PlayerController.OnPlayerDie += PlayerController_OnPlayerDie;
        PlayerController.OnLevelUp += PlayerController_OnLevelUp;
    }

    private void OnDisable()
    {
        input.TabButtonAction -= TabButtonAction;
        PlayerController.OnPlayerDie -= PlayerController_OnPlayerDie;
        PlayerController.OnLevelUp -= PlayerController_OnLevelUp;
    }

    private void Start()
    {
        input.EnableMenuActions();
        SetCursorState(true);
    }

    private void PlayerController_OnPlayerDie()
    {
        gameOverView.Open();
        crossHair.SetActive(false);
        SetCursorState(false);
    }

    private void PlayerController_OnLevelUp()
    {
        OpenTalentTree();
    }

    private void TabButtonAction()
    {
        bool active = talentTreeView.gameObject.activeSelf;

        if (active)
            CloseTalentTree();
        else
            OpenTalentTree();
    }

    private void OpenTalentTree() 
    {
        input.DisablePlayerActions();
        talentTreeView.Open();
        crossHair.SetActive(false);
        SetCursorState(false);

        //Refactor
        Time.timeScale = 0;
    }

    private void CloseTalentTree() 
    {
        input.EnablePlayerActions();
        talentTreeView.Close();
        crossHair.SetActive(true);
        SetCursorState(true);

        //Refactor
        Time.timeScale = 1;
    }

    private void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
}
