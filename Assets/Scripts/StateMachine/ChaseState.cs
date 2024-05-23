using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : EnemyBaseState
{
    readonly NavMeshAgent agent;

    public ChaseState(EnemyController enemy, UnitAnimationBehaviour animationBehaviour, NavMeshAgent agent) : base(enemy, animationBehaviour)
    {
        this.agent = agent;
    }

    public override void OnEnter()
    {
        agent.speed = enemy.Config.RunSpeed;
        animationBehaviour.Run();
    }

    public override void Update()
    {
        if(PlayerController.Player)
            agent.SetDestination(PlayerController.Player.position);
    }
}
