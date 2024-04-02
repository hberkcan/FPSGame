using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerController : MonoBehaviour
{
    private PlayerInputController inputs;
    private CharacterController controller;

    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveSpeedMultiplier = 2f;
    [SerializeField] private float rotationSpeed = 2;

    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -15f;

    [SerializeField] private bool grounded = true;
    [SerializeField] private float groundedOffset = 0.15f;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private float bottomClamp = -90f;
    [SerializeField] private float topClamp = 60f;

    private float verticalVelocity;
    private float rotationVelocity;
    private float terminalVelocity = 50f;

    private float cameraTargetPitch;

    private const float threshold = 0f;

    [SerializeField] private Gun gun;

    private PlayerTalents playerTalents;

    private void Awake()
    {
        inputs = GetComponent<PlayerInputController>();
        controller = GetComponent<CharacterController>();
        playerTalents = GetComponent<PlayerTalents>();
        gun = GetComponentInChildren<Gun>();
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
        float targetSpeed = inputs.Sprint ? moveSpeed * moveSpeedMultiplier : moveSpeed;

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

    public void UpgradeMaxHealth(int value)
    {
        maxHealth += value;
    }

    public void UpgradeMoveSpeed(float value) 
    {
        moveSpeed += value;
    }

    public void UpgradeJumpHeight(float value) 
    {
        jumpHeight += value;
    }
}
