using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private EnemyConfig config;

    private NavMeshAgent agent;
    private NavMeshHit hit;

    private CountdownTimer detectionTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        detectionTimer = new CountdownTimer(config.DetectionCooldown);
    }

    void Update() => detectionTimer.Tick(Time.deltaTime);

    public bool CanDetectPlayer()
    {
        if(!PlayerController.Player)
            return false;

        return detectionTimer.IsRunning || Detect();
    }

    public bool CanAttackPlayer()
    {
        if (!PlayerController.Player)
            return false;

        if(!IsPlayerOnSight())
            return false;

        var directionToPlayer = PlayerController.Player.position - transform.position;
        return directionToPlayer.magnitude <= config.AttackRange;
    }

    private bool IsPlayerOnSight()
    {
        if (!agent.Raycast(PlayerController.Player.position, out hit))
        {
            return true; // Target is "visible" from our position.
        }

        return false;
    }

    public bool Detect()
    {
        if (detectionTimer.IsRunning) return false;

        var directionToPlayer = PlayerController.Player.position - transform.position;
        var angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);

        // If the player is not within the detection angle + outer radius (aka the cone in front of the enemy),
        // or is within the inner radius, return false
        if ((!(angleToPlayer < config.DetectionAngle / 2f) || !(directionToPlayer.magnitude < config.DetectionRadius))
            && !(directionToPlayer.magnitude < config.InnerDetectionRadius))
            return false;

        detectionTimer.Start();
        return true;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, config.DetectionRadius);
        Gizmos.DrawWireSphere(transform.position, config.InnerDetectionRadius);

        Vector3 forwardConeDirection = Quaternion.Euler(0, config.DetectionAngle / 2, 0) * transform.forward * config.DetectionRadius;
        Vector3 backwardConeDirection = Quaternion.Euler(0, -config.DetectionAngle / 2, 0) * transform.forward * config.DetectionRadius;

        Gizmos.DrawLine(transform.position, transform.position + forwardConeDirection);
        Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
    }
}
