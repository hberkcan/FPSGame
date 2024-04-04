using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private Transform player;
    [SerializeField] private EnemyConfig config;
    private CountdownTimer detectionTimer;

    public void Initialize(Transform player) 
    {
        this.player = player;
    }

    void Start()
    {
        detectionTimer = new CountdownTimer(config.detectionCooldown);
    }

    void Update() => detectionTimer.Tick(Time.deltaTime);

    public bool CanDetectPlayer()
    {
        return detectionTimer.IsRunning || Detect();
    }

    public bool CanAttackPlayer()
    {
        var directionToPlayer = player.position - transform.position;
        return directionToPlayer.magnitude <= config.attackRange;
    }

    public bool Detect()
    {
        if (detectionTimer.IsRunning) return false;

        var directionToPlayer = player.position - transform.position;
        var angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);

        // If the player is not within the detection angle + outer radius (aka the cone in front of the enemy),
        // or is within the inner radius, return false
        if ((!(angleToPlayer < config.detectionAngle / 2f) || !(directionToPlayer.magnitude < config.detectionRadius))
            && !(directionToPlayer.magnitude < config.innerDetectionRadius))
            return false;

        detectionTimer.Start();
        return true;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, config.detectionRadius);
        Gizmos.DrawWireSphere(transform.position, config.innerDetectionRadius);

        Vector3 forwardConeDirection = Quaternion.Euler(0, config.detectionAngle / 2, 0) * transform.forward * config.detectionRadius;
        Vector3 backwardConeDirection = Quaternion.Euler(0, -config.detectionAngle / 2, 0) * transform.forward * config.detectionRadius;

        Gizmos.DrawLine(transform.position, transform.position + forwardConeDirection);
        Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
    }
}
