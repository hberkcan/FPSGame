using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;

    public ChaseState(EnemyController enemy, UnitAnimationBehaviour animationBehaviour, NavMeshAgent agent, Transform player) : base(enemy, animationBehaviour)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        agent.speed = enemy.Config.RunSpeed;
        animationBehaviour.Run();
    }

    public override void OnExit()
    {
        //
    }

    public override void Update()
    {
        agent.SetDestination(player.position);

        if (!enemy.CanDetectPlayer()) 
        {
            enemy.ChangeState(EnemyState.Wander);
            return;
        }

        if (enemy.CanAttackPlayer())
            enemy.ChangeState(EnemyState.Attack);
    }
}
