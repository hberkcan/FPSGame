using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LingerState : EnemyBaseState
{
    private readonly NavMeshAgent agent;

    public LingerState(EnemyController enemy, UnitAnimationBehaviour animationBehaviour, NavMeshAgent agent) : base(enemy, animationBehaviour)
    {
        this.agent = agent;
    }

    public override void OnEnter()
    {
        animationBehaviour.Idle();
        agent.speed = 0;

        enemy.OnLinger();
    }
}
