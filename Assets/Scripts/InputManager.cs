using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private GameInputs inputs;

    private const float buttonPressPoint = 0.5f;

    [SerializeField] private bool cursorLocked = true;
    [SerializeField] private bool cursorInputForLook = true;

    private void OnEnable()
    {
        inputs = new GameInputs();
        inputs.Enable();

        inputs.Player.Move.performed += OnMove;
        inputs.Player.Look.performed += OnLook;
        inputs.Player.Sprint.performed += OnSprint;
        inputs.Player.Jump.performed += OnJump;
        inputs.Player.Fire.performed += OnFire;

        inputs.Menu.TalentTreeMenu.performed += OnTalentTreeButtonPressed;
    }

    private void Start()
    {
        SetCursorState(cursorLocked);
    }

    private void OnTalentTreeButtonPressed(InputAction.CallbackContext context)
    {
        MenuInputs.IsTalentTreeViewOpen.Value = !MenuInputs.IsTalentTreeViewOpen.Value;

        if (MenuInputs.IsTalentTreeViewOpen.Value) 
        {
            inputs.Player.Disable();
            SetCursorState(false);
        }
        else 
        {
            inputs.Player.Enable();
            SetCursorState(true);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        PlayerInputs.Move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (cursorInputForLook)
            PlayerInputs.Look = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        PlayerInputs.IsSprinting = context.ReadValue<float>() >= buttonPressPoint;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        PlayerInputs.IsJumping = context.ReadValue<float>() >= buttonPressPoint;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        PlayerInputs.IsFiring.Value = context.ReadValue<float>() >= buttonPressPoint;
    }

    public void OnFireCanceled(InputAction.CallbackContext context)
    {
        PlayerInputs.IsFiring.Value = context.ReadValue<float>() >= buttonPressPoint;
    }

    private void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }

    public static class PlayerInputs
    {
        public static Vector2 Move { get; internal set; }
        public static Vector2 Look { get; internal set; }
        public static Observer<bool> IsFiring { get; set; } = new Observer<bool>(false);
        public static bool IsSprinting { get; internal set; }
        public static bool IsJumping { get; set; }
    }

    public static class MenuInputs
    {
        public static Observer<bool> IsTalentTreeViewOpen { get; internal set; } = new Observer<bool>(false);
    }
}
