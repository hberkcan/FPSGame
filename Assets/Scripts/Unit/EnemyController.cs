using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(UnitAnimationBehaviour))]
[RequireComponent(typeof(PlayerDetector))]
public class EnemyController : MonoBehaviour, IDamagable
{
    private NavMeshAgent agent;
    private UnitAnimationBehaviour animationBehaviour;
    private PlayerDetector playerDetector;

    [SerializeField] private EnemyConfig config;
    public EnemyConfig Config => config;

    private int currentHealth;
    private CountdownTimer attackTimer;
    private bool canAttack = true;

    private readonly Dictionary<EnemyState, IState> states = new Dictionary<EnemyState, IState>();
    private IState currentState;

    private Transform player;

    [SerializeField] private Gun gun;
    [SerializeField] private bool overrideGunDamage;

    public UnityEvent<float> OnHealthChange;
    public UnityEvent OnDie;
    public static event Action<EnemyConfig> OnAnyEnemyDie;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animationBehaviour = GetComponent<UnitAnimationBehaviour>();
        playerDetector = GetComponent<PlayerDetector>();

        attackTimer = new CountdownTimer(config.TimeBetweenAttacks);
        attackTimer.OnTimerStop = AttackCooldown;
    }

    private void Start()
    {
        currentHealth = config.Health;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerDetector.Initialize(player);

        if(overrideGunDamage) 
        {
            gun.SetDamage(config.AttackDamage);
        }

        var lingerState = new LingerState(this, animationBehaviour, agent);
        var wanderState = new WanderState(this, animationBehaviour, agent, config.WanderRadius);
        var chaseState = new ChaseState(this, animationBehaviour, agent, player);
        var attackState = new AttackState(this, animationBehaviour, agent, player);
        var deadState = new DeadState(this, animationBehaviour, agent);

        states.Add(EnemyState.Linger, lingerState);
        states.Add(EnemyState.Wander, wanderState);
        states.Add(EnemyState.Chase, chaseState);
        states.Add(EnemyState.Attack, attackState);
        states.Add(EnemyState.Dead, deadState);

        ChangeState(EnemyState.Linger);
    }

    private void Update()
    {
        currentState.Update();
        attackTimer.Tick(Time.deltaTime);
    }

    public void ChangeState(EnemyState state) 
    {
        currentState?.OnExit();
        currentState = states[state];
        currentState.OnEnter();
    }

    public void Attack() 
    {
        FaceTarget();

        if (!attackTimer.IsRunning && canAttack) 
        {
            attackTimer.Start();
        }

        if (!canAttack)
            return;

        gun.Use();
    }

    private void AttackCooldown() => StartCoroutine(AttackCooldownCoroutine());
    private IEnumerator AttackCooldownCoroutine()
    {
        canAttack = false;
        yield return Helpers.GetWaitForSeconds(config.TimeBetweenAttacks);
        canAttack = true;
    }

    private void FaceTarget() 
    {
        Quaternion lookRotation = Quaternion.LookRotation(player.position - transform.position);
        float rotationSpeed = 2.5f;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth == 0)
        {
            OnDie?.Invoke();
            OnAnyEnemyDie?.Invoke(config);
            ChangeState(EnemyState.Dead);
        }

        OnHealthChange?.Invoke(GetHealthPercentage());
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / config.Health;
    }

    public float GetRandomLingerTime() 
    {
        float min = 2f;

        return Random.Range(min, config.LingerTime);
    }

    public bool CanDetectPlayer() 
    {
        return playerDetector.CanDetectPlayer();
    }

    public bool CanAttackPlayer() 
    {
        return playerDetector.CanAttackPlayer();
    }
}
