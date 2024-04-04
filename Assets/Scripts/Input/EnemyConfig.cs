using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    public int Health = 100;
    public int AttackDamage = 2;
    public float WalkSpeed = 2.2f;
    public float RunSpeed = 3f;
    public int EXPReward = 10;
    public float LingerTime = 2;
    public float WanderRadius = 10f;
    public float TimeBetweenAttacks = 0.5f;
    public float detectionAngle = 90f;
    public float detectionRadius = 25f;
    public float innerDetectionRadius = 10f;
    public float detectionCooldown = 0.5f;
    public float attackRange = 15f;
}
