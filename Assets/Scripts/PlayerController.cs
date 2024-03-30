using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private FPSInputs inputs;
    private CharacterController controller;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float rotationSpeed = 1.5f;

    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -15f;

    [SerializeField] private bool grounded = true;
    [SerializeField] private float groundedOffset = 0.15f;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private float bottomClamp = -90f;
    [SerializeField] private float topClamp = 90f;

    private float verticalVelocity;
    private float rotationVelocity;
    private float terminalVelocity = 50f;

    private float cameraTargetPitch;

    private const float threshold = 0f;

    public static event Action<bool> IsShooting;

    private Gun gun;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        inputs = GetComponent<FPSInputs>();
        controller = GetComponent<CharacterController>();
        gun = GetComponentInChildren<Gun>();

        playerInput.currentActionMap.FindAction("Fire").started += PlayerController_started;
        playerInput.currentActionMap.FindAction("Fire").canceled += PlayerController_canceled;
    }

    private void PlayerController_canceled(InputAction.CallbackContext obj)
    {
        inputs.Fire = false;
        IsShooting?.Invoke(false);
    }

    private void PlayerController_started(InputAction.CallbackContext obj)
    {
        inputs.Fire = true;
        IsShooting?.Invoke(true);
    }

    private void Update()
    {
        HandleMovement();
        HandleJumpAndGravity();
        GroundedCheck();

        Shoot();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void Shoot()
    {
        if(inputs.Fire)
            gun.Use();
    }

    private void HandleMovement()
    {
        float targetSpeed = inputs.Sprint ? sprintSpeed : moveSpeed;

        if(inputs.Move == Vector2.zero)
            targetSpeed = 0;


        Vector3 inputDirection = new(inputs.Move.x, 0.0f, inputs.Move.y);

        if (inputs.Move != Vector2.zero)
        {
            inputDirection = transform.right * inputs.Move.x + transform.forward * inputs.Move.y;
        }

        controller.Move((targetSpeed * inputDirection + new Vector3(0, verticalVelocity, 0)) * Time.deltaTime);
    }

    private void HandleJumpAndGravity()
    {
        if (grounded)
        {
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -5f;
            }

            if (inputs.Jump)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            inputs.Jump = false;
        }

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, controller.radius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    private void CameraRotation()
    {
        if (inputs.Look.sqrMagnitude >= threshold)
        {
            cameraTargetPitch += inputs.Look.y * rotationSpeed;
            rotationVelocity = inputs.Look.x * rotationSpeed;

            cameraTargetPitch = Helpers.ClampAngle(cameraTargetPitch, bottomClamp, topClamp);

            cameraTarget.transform.localRotation = Quaternion.Euler(cameraTargetPitch, 0.0f, 0.0f);

            transform.Rotate(Vector3.up * rotationVelocity);
        }
    }
}
