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

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction fireAction;
    private const float buttonPressPoint = 0.5f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
        jumpAction = playerInput.actions.FindAction("Jump");
        sprintAction = playerInput.actions.FindAction("Sprint");
        fireAction = playerInput.actions.FindAction("Fire");


        moveAction.performed += OnMove;
        lookAction.performed += OnLook;
        jumpAction.performed += OnJump;
        sprintAction.performed += OnSprint;
        fireAction.performed += OnFire;
        fireAction.canceled += OnFireCanceled;
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

    public void OnSprint(InputAction.CallbackContext context)
    {
        Sprint = context.ReadValue<float>() >= buttonPressPoint;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump = context.ReadValue<float>() >= buttonPressPoint;
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

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
