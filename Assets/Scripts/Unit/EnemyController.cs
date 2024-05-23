using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
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
    private bool canAttack = true;

    private StateMachine stateMachine;
    private readonly Dictionary<EnemyState, IState> states = new Dictionary<EnemyState, IState>();

    [SerializeField] private Gun gun;
    [SerializeField] private bool overrideGunDamage;

    public UnityEvent<float> OnHealthChange;
    public UnityEvent OnDie;
    public static event Action<EnemyConfig> OnAnyEnemyDie;

    private List<Timer> timers;
    private CountdownTimer attackTimer;
    private CountdownTimer lingerTimer;
    private CountdownTimer agroTimer;

    private bool hasAgro;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animationBehaviour = GetComponent<UnitAnimationBehaviour>();
        playerDetector = GetComponent<PlayerDetector>();

        SetupTimers();
        SetupStateMachine();
    }

    private void Start()
    {
        currentHealth = config.Health;

        if(overrideGunDamage) 
        {
            gun.SetDamage(config.AttackDamage);
        }

        stateMachine.SetState(states[EnemyState.Linger]);
    }

    private void Update()
    {
        HandleTimers();
        stateMachine.Update();
    }

    private void SetupStateMachine() 
    {
        stateMachine = new StateMachine();

        var lingerState = new LingerState(this, animationBehaviour, agent);
        var wanderState = new WanderState(this, animationBehaviour, agent, config.WanderRadius);
        var chaseState = new ChaseState(this, animationBehaviour, agent);
        var attackState = new AttackState(this, animationBehaviour, agent);
        var deadState = new DeadState(this, animationBehaviour, agent);

        states.Add(EnemyState.Linger, lingerState);
        states.Add(EnemyState.Wander, wanderState);
        states.Add(EnemyState.Chase, chaseState);
        states.Add(EnemyState.Attack, attackState);
        states.Add(EnemyState.Dead, deadState);

        AddTransition(lingerState, wanderState, new FuncPredicate(() => !lingerTimer.IsRunning));
        AddTransition(wanderState, lingerState, new FuncPredicate(() => HasReachedDestination()));
        AddTransition(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
        AddTransition(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer() && !hasAgro));
        AddTransition(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
        AddTransition(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));

        AddAnyTransition(chaseState, new FuncPredicate(() => isAlive && hasAgro && !playerDetector.CanAttackPlayer()));
        AddAnyTransition(attackState, new FuncPredicate(() => isAlive && hasAgro && playerDetector.CanAttackPlayer()));
        AddAnyTransition(deadState, new FuncPredicate(() => !isAlive));
    }

    private void SetupTimers() 
    {
        attackTimer = new CountdownTimer(config.TimeBetweenAttacks);
        attackTimer.OnTimerStop = AttackCooldown;

        lingerTimer = new CountdownTimer(GetRandomLingerTime());

        agroTimer = new CountdownTimer(config.AgroTime);
        agroTimer.OnTimerStart = () => hasAgro = true;
        agroTimer.OnTimerStop = () => hasAgro = false;

        timers = new List<Timer>(3) { attackTimer, lingerTimer, agroTimer };
    }

    private void HandleTimers() 
    {
        foreach (Timer timer in timers) 
        {
            timer.Tick(Time.deltaTime);
        }
    }

    private void AddTransition(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    private void AddAnyTransition(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

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
        Quaternion lookRotation = Quaternion.LookRotation(PlayerController.Player.position - transform.position);
        float rotationSpeed = 5f;
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
        }

        OnHealthChange?.Invoke(GetHealthPercentage());

        if (!agroTimer.IsRunning)
            agroTimer.Start();
        else
            agroTimer.Reset();
    }

    private float GetHealthPercentage()
    {
        return (float)currentHealth / config.Health;
    }

    private float GetRandomLingerTime() 
    {
        float min = 2f;

        return Random.Range(min, config.LingerTime);
    }

    private bool HasReachedDestination()
    {
        return !agent.pathPending
                   && agent.remainingDistance <= agent.stoppingDistance
                   && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }

    private bool isAlive => currentHealth > 0;

    public void OnLinger() 
    {
        lingerTimer.Reset(GetRandomLingerTime());
        lingerTimer.Start();
    }

    public void DestroyAfterDie() 
    {
        float delay = 2f;
        Destroy(gameObject, delay);
    }
}
