using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeadState : EnemyBaseState
{
    readonly NavMeshAgent agent;

    public DeadState(EnemyController enemy, UnitAnimationBehaviour animationBehaviour, NavMeshAgent agent) : base(enemy, animationBehaviour)
    {
        this.agent = agent;
    }

    public override void OnEnter()
    {
        agent.enabled = false;
        animationBehaviour.Die();
        enemy.DestroyAfterDie();
    }
}
