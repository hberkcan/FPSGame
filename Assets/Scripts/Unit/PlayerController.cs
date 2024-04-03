using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamagable
{
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
    private float cameraTargetPitch;

    private float verticalVelocity;
    private float rotationVelocity;
    private readonly float terminalVelocity = 50f;

    private const float threshold = 0f;

    [SerializeField] private Gun gun;

    public event Action OnDie;
    public event Action<int> OnTakeDamage;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
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
        if(InputManager.PlayerInputs.IsFiring.Value)
            gun.Use();
    }

    private void HandleMovement()
    {
        float targetSpeed = InputManager.PlayerInputs.IsSprinting ? moveSpeed * moveSpeedMultiplier : moveSpeed;

        if(InputManager.PlayerInputs.Move == Vector2.zero)
            targetSpeed = 0;

        Vector3 inputDirection = new(InputManager.PlayerInputs.Move.x, 0.0f, InputManager.PlayerInputs.Move.y);

        if (InputManager.PlayerInputs.Move != Vector2.zero)
        {
            inputDirection = transform.right * InputManager.PlayerInputs.Move.x + transform.forward * InputManager.PlayerInputs.Move.y;
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

            if (InputManager.PlayerInputs.IsJumping)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            InputManager.PlayerInputs.IsJumping = false;
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
        if (InputManager.PlayerInputs.Look.sqrMagnitude >= threshold)
        {
            cameraTargetPitch += InputManager.PlayerInputs.Look.y * rotationSpeed;
            rotationVelocity = InputManager.PlayerInputs.Look.x * rotationSpeed;

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

    public void UpgradeGunDamage(int value) 
    {
        gun.UpgradeDamage(value);
    }

    public void UpgradeGunAmmoCapacity(int value) 
    {
        gun.UpgradeAmmoCapacity(value);
    }

    public void UpgradeGunPierce(bool value) 
    {
        gun.CanPierce = value;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth == 0)
        {
            OnDie?.Invoke();
        }

        OnTakeDamage?.Invoke(-damage);
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public void AddHealth(int health)
    {
        currentHealth += health;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    public void AddAmmo(int ammo) 
    {
        gun.AddAmmo(ammo);
    }
}
