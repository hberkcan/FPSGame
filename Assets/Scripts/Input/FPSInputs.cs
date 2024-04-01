using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSInputs : MonoBehaviour
{
    [field: SerializeField] public Vector2 Move { get; private set; }
    [field: SerializeField] public Vector2 Look {  get; private set; }
    [field: SerializeField] public bool Fire {  get; set; }
    [field: SerializeField] public bool Sprint {  get; private set; }
    [field: SerializeField] public bool Jump {  get; set; }

    [SerializeField] private bool cursorLocked = true;
    [SerializeField] private bool cursorInputForLook = true;

    public void OnMove(InputValue value)
    {
        Move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        if(cursorInputForLook)
            Look = value.Get<Vector2>();
    }

    public void OnFire(InputValue value)
    {
        Fire = value.isPressed;
    }

    public void OnSprint(InputValue value)
    {
        Sprint = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        Jump = value.isPressed;
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
