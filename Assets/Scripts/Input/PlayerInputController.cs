using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [field: SerializeField] public Vector2 Move { get; private set; }
    [field: SerializeField] public Vector2 Look {  get; private set; }
    [field: SerializeField] public bool Fire {  get; set; }
    [field: SerializeField] public bool Sprint {  get; private set; }
    [field: SerializeField] public bool Jump {  get; set; }

    public static event Action<bool> IsShooting;

    [SerializeField] private bool cursorLocked = true;
    [SerializeField] private bool cursorInputForLook = true;

    FPSInputs inputActions;

    private const float buttonPressPoint = 0.5f;

    private void OnEnable()
    {
        inputActions = new FPSInputs();
        inputActions.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Fire.performed += OnFire;
        inputActions.Player.Fire.canceled += OnFireCanceled;
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Sprint.performed += OnSprint;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if(cursorInputForLook)
            Look = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        Fire = context.ReadValue<float>() >= buttonPressPoint;
        IsShooting?.Invoke(true);
    }

    public void OnFireCanceled(InputAction.CallbackContext context)
    {
        Fire = context.ReadValue<float>() >= buttonPressPoint;
        IsShooting?.Invoke(false);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        Sprint = context.ReadValue<float>() >= buttonPressPoint;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump = context.ReadValue<float>() >= buttonPressPoint;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        //SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
