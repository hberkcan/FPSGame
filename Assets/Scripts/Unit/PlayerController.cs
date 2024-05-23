using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem.HID;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamagable
{
    public static Transform Player {  get; private set; }
    private CharacterController controller;
    private Recoil recoil;

    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

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
    public int CurrentAmmo => gun.CurrentAmmo;
    public int MaxAmmo => gun.MaxAmmo;

    [SerializeField] private LevelSystem levelSystem;
    public int CurrentLevel => levelSystem.CurrentLevel;
    public float XPPercentage => levelSystem.XPPercentage;

    private int killScore = 0;
    public int KillScore => killScore;

    public static event Action OnPlayerDie;
    public static event Action<int, float> OnHealthChange;
    public event Action OnXPGain;
    public event Action OnAmmoChange;
    public event Action OnGetKill;
    public static event Action OnLevelUp;

    [SerializeField] private InputReader input;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        gun = GetComponentInChildren<Gun>();
        recoil = GetComponent<Recoil>();

        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        Player = transform;
        gun.OnShoot += Gun_OnShoot;
        EnemyController.OnAnyEnemyDie += EnemyController_OnAnyEnemyDie;
        input.Jump += OnJump;
    }

    private void OnDisable()
    {
        EnemyController.OnAnyEnemyDie -= EnemyController_OnAnyEnemyDie;
        input.Jump -= OnJump;
    }

    private void Start()
    {
        input.EnablePlayerActions();
    }

    private void Update()
    {
        if (!controller.enabled)
            return;

        HandleMovement();
        HandleJumpAndGravity();
        GroundedCheck();
        HandleShoot();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ICollectable collectible))
        {
            collectible.OnCollect(this);
        }
    }

    private void OnJump(bool performed) 
    {
        if (performed && grounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void HandleShoot()
    {
        if (input.IsFiring) 
        {
            gun.Use();
        }
    }

    private void HandleMovement()
    {
        float targetSpeed = input.IsSprinting ? moveSpeed * moveSpeedMultiplier : moveSpeed;

        if(input.MoveDirection == Vector3.zero)
            targetSpeed = 0;

        Vector3 inputDirection = new(input.MoveDirection.x, 0.0f, input.MoveDirection.y);

        if (input.MoveDirection != Vector3.zero)
        {
            inputDirection = transform.right * input.MoveDirection.x + transform.forward * input.MoveDirection.y;
        }

        Vector3 target = (targetSpeed * inputDirection + new Vector3(0, verticalVelocity, 0)) * Time.deltaTime;
        controller.Move(target);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -50, 50);
        pos.z = Mathf.Clamp(pos.z, -50, 50);
        transform.position = pos;
    }

    private void HandleJumpAndGravity()
    {
        if (grounded)
        {
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -5f;
            }

            //if (InputManager.PlayerInputs.IsJumping)
            //{
            //    //required velocity for desired height
            //    verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            //}
        }
        //else
        //{
        //    InputManager.PlayerInputs.IsJumping = false;
        //}

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
        if (input.LookDirection.sqrMagnitude >= threshold)
        {
            cameraTargetPitch += input.LookDirection.y * rotationSpeed;
            rotationVelocity = input.LookDirection.x * rotationSpeed;

            cameraTargetPitch = Helpers.ClampAngle(cameraTargetPitch, bottomClamp, topClamp);

            cameraTarget.transform.localRotation = Quaternion.Euler(cameraTargetPitch, 0, 0);

            transform.Rotate(Vector3.up * rotationVelocity);
        }
    }

    public void UpgradeMaxHealth(int value)
    {
        float previousPercentage = GetHealthPercentage();
        int previousCurrentHealth = currentHealth;
        maxHealth += value;
        currentHealth = Mathf.CeilToInt(maxHealth * previousPercentage);
        OnHealthChange?.Invoke(previousCurrentHealth - currentHealth, GetHealthPercentage());
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
        CameraShake.Instance.RequestShake(CameraShake.ShakeType.Damage, 0.4f, 0.15f);

        if (currentHealth == 0)
        {
            Die();
        }

        OnHealthChange?.Invoke(-damage, GetHealthPercentage());
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public void AddHealth(int health)
    {
        currentHealth += health;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChange?.Invoke(health, GetHealthPercentage());
    }

    public void AddAmmo(int ammo) 
    {
        gun.AddAmmo(ammo);
        OnAmmoChange?.Invoke();
    }

    public void AddExperience(int experience) 
    {
        if (levelSystem.AddXP(experience))
            OnLevelUp?.Invoke();

        OnXPGain?.Invoke();
    }

    private void Gun_OnShoot()
    {
        float focusMultiplier = 1f;

        if (input.IsFocusing)
            focusMultiplier = 0.35f;

        recoil.RecoilFire(gun.Spread * focusMultiplier);
        OnAmmoChange?.Invoke();
        CameraShake.Instance.RequestShake(CameraShake.ShakeType.Recoil, 0.2f, 0.1f);
    }

    private void EnemyController_OnAnyEnemyDie(EnemyConfig config)
    {
        AddExperience(config.EXPReward);
        killScore++;
        OnGetKill?.Invoke();
    }

    private void Die() 
    {
        OnPlayerDie?.Invoke();
        Player = null;
        controller.enabled = false;
        input.DisablePlayerActions();

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        float torqueMagnitude = 0.8f;
        rb.AddTorque(transform.forward * torqueMagnitude, ForceMode.Impulse);

        gun.transform.parent = null;
        gun.AddComponent<Rigidbody>();
        gun.AddComponent<BoxCollider>();
    }
}
