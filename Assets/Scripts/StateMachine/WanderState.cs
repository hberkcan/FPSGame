using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Vector3 startPoint;
    readonly float wanderRadius;
    Vector3 destination;

    public WanderState(EnemyController enemy, UnitAnimationBehaviour animationBehaviour, NavMeshAgent agent, float wanderRadius) : base(enemy, animationBehaviour) 
    {
        this.agent = agent;
        this.startPoint = enemy.transform.position;
        this.wanderRadius = wanderRadius;
    }

    public override void OnEnter()
    {
        agent.speed = enemy.Config.WalkSpeed;
        animationBehaviour.Walk();

        var randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += startPoint;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
        destination = hit.position;
    }

    public override void OnExit()
    {
        //
    }

    public override void Update()
    {
        agent.SetDestination(destination);

        if (enemy.CanDetectPlayer()) 
        {
            enemy.ChangeState(EnemyState.Chase);
            return;
        }

        if (HasReachedDestination())
            enemy.ChangeState(EnemyState.Linger);
    }

    private bool HasReachedDestination()
    {
        return !agent.pathPending
                   && agent.remainingDistance <= agent.stoppingDistance
                   && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }
}
