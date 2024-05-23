using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject, GameInputs.IPlayerActions, GameInputs.IMenuActions
{
    private GameInputs gameInputs;

    //PlayerActions
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<Vector2> Look = delegate { };
    public event UnityAction<bool> Sprint = delegate { };
    public event UnityAction<bool> Jump = delegate { };
    public event UnityAction<bool> Fire = delegate { };
    public event UnityAction<bool> Focus = delegate { };

    //MenuActions
    public event UnityAction TabButtonAction = delegate { };

    public Vector3 MoveDirection => gameInputs.Player.Move.ReadValue<Vector2>();
    public Vector3 LookDirection => gameInputs.Player.Look.ReadValue<Vector2>();
    public bool IsFiring {  get; private set; }
    public bool IsSprinting {  get; private set; }
    public bool IsFocusing {  get; private set; }

    private const float buttonPressPoint = 0.5f;

    private void OnEnable()
    {
        if(gameInputs == null) 
        {
            gameInputs = new GameInputs();
            gameInputs.Player.SetCallbacks(this);
            gameInputs.Menu.SetCallbacks(this);
        }
    }

    public void EnablePlayerActions() => gameInputs.Player.Enable();
    public void DisablePlayerActions() => gameInputs.Player.Disable();
    public void EnableMenuActions() => gameInputs.Menu.Enable();
    public void DisableMenuActions() => gameInputs.Menu.Disable();

    public void OnFire(InputAction.CallbackContext context)
    {
        Fire.Invoke(context.ReadValue<float>() >= buttonPressPoint);

        IsFiring = context.started || context.performed;
    }

    public void OnFocus(InputAction.CallbackContext context)
    {
        Focus.Invoke(context.ReadValue<float>() >= buttonPressPoint);

        IsFocusing = context.started || context.performed;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump.Invoke(context.ReadValue<float>() >= buttonPressPoint);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Look.Invoke(context.ReadValue<Vector2>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        Sprint.Invoke(context.ReadValue<float>() >= buttonPressPoint);

        IsSprinting = context.started || context.performed;
    }

    public void OnTab(InputAction.CallbackContext context)
    {
        TabButtonAction.Invoke();
    }
}
