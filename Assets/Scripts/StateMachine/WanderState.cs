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
        startPoint = enemy.transform.position;
        this.wanderRadius = wanderRadius;
    }

    public override void OnEnter()
    {
        agent.speed = enemy.Config.WalkSpeed;
        animationBehaviour.Walk();
        destination = GetRandomPoint();
    }

    public override void Update()
    {
        agent.SetDestination(destination);
    }

    private Vector3 GetRandomPoint()
    {
        var randomDirection = startPoint + Random.insideUnitSphere * wanderRadius;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
        return hit.position;
    }
}
