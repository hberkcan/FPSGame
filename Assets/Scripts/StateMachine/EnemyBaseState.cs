using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState 
{
    Linger,
    Wander,
    Chase,
    Attack,
    Dead
}
public abstract class EnemyBaseState : IState
{
    protected readonly EnemyController enemy;
    protected readonly UnitAnimationBehaviour animationBehaviour;

    protected EnemyBaseState(EnemyController enemy, UnitAnimationBehaviour animationBehaviour)
    {
        this.enemy = enemy;
        this.animationBehaviour = animationBehaviour;
    }

    public virtual void OnEnter()
    {
        //
    }

    public virtual void OnExit()
    {
        //
    }

    public virtual void Update()
    {
        //
    }
}
