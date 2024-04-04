using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LingerState : EnemyBaseState
{
    private readonly NavMeshAgent agent;
    private float desiredLingerTime;
    private float lingerTime;

    public LingerState(EnemyController enemy, UnitAnimationBehaviour animationBehaviour, NavMeshAgent agent) : base(enemy, animationBehaviour)
    {
        this.agent = agent;
    }

    public override void OnEnter()
    {
        animationBehaviour.Idle();
        desiredLingerTime = enemy.GetRandomLingerTime();
        agent.speed = 0;
    }

    public override void Update()
    {
        lingerTime += Time.deltaTime;

        if(lingerTime >= desiredLingerTime) 
        {
            enemy.ChangeState(EnemyState.Wander);
        }
    }

    public override void OnExit()
    {
        lingerTime = 0;
    }
}
