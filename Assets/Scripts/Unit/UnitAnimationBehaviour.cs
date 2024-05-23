using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimationBehaviour : MonoBehaviour
{
    private Animator animator;

    static readonly int IdleHash = Animator.StringToHash("Rifle_Idle");
    static readonly int WalkHash = Animator.StringToHash("Rifle_Walk");
    static readonly int RunHash = Animator.StringToHash("Rifle_Run");
    static readonly int AttackHash = Animator.StringToHash("Rifle_Fire");
    static readonly int HitHash = Animator.StringToHash("Rifle_Hit");
    static readonly int DieHash = Animator.StringToHash("Rifle_Die");
    const float crossFadeDuration = 0.1f;

    private readonly Dictionary<int, float> animationDuration = new()
    {
        { IdleHash, 0.5f },
        { WalkHash, 0.5f },
        { RunHash, 0.5f },
        { AttackHash, 0f },
        { HitHash, 0f },
        { DieHash, 0.5f }
    };

    private void Awake() => animator = GetComponent<Animator>();

    private float PlayAnimation(int animationHash)
    {
        animator.CrossFade(animationHash, crossFadeDuration);
        return animationDuration[animationHash];
    }

    //public void UpdateAnimator(float velocity) => animator.SetFloat("Velocity", velocity);
    public float Idle() => PlayAnimation(IdleHash);
    public float Walk() => PlayAnimation(WalkHash);
    public float Run() => PlayAnimation(RunHash);
    public float Attack() => PlayAnimation(AttackHash);
    public float Hit() => PlayAnimation(HitHash);
    public float Die() => PlayAnimation(DieHash);
}
