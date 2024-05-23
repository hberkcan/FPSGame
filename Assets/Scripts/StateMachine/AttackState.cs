using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : EnemyBaseState
{
    readonly NavMeshAgent agent;

    public AttackState(EnemyController enemy, UnitAnimationBehaviour animationBehaviour, NavMeshAgent agent) : base(enemy, animationBehaviour)
    {
        this.agent = agent;
    }

    public override void OnEnter()
    {
        animationBehaviour.Attack();
        agent.isStopped = true;
    }

    public override void OnExit()
    {
        agent.isStopped = false;
    }

    public override void Update()
    {
        enemy.Attack();
    }
}
