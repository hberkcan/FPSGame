using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;

    public AttackState(EnemyController enemy, UnitAnimationBehaviour animationBehaviour, NavMeshAgent agent, Transform player) : base(enemy, animationBehaviour)
    {
        this.agent = agent;
        this.player = player;
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

        if (!enemy.CanAttackPlayer())
            enemy.ChangeState(EnemyState.Wander);
    }
}
