using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileBehaviour
{
    public void MoveProjectile(Rigidbody projectile);
}

public class LinearProjectileBehaviour : IProjectileBehaviour
{
    private Vector3 direction;
    private float speed;

    public LinearProjectileBehaviour(Vector3 direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
    }

    public void MoveProjectile(Rigidbody projectile)
    {
        projectile.MovePosition(projectile.transform.position + speed * Time.deltaTime * direction);
        projectile.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x)));
    }
}

public class OrbitalProjectileBehaviour : IProjectileBehaviour
{
    private Transform orbit;
    private float radius;
    private float rotationSpeed;
    private Vector3 startPos;
    private Vector3 orbitStart;
    private float angle = 0;

    public OrbitalProjectileBehaviour(Transform orbit, Vector3 startPos, float radius, float rotationSpeed)
    {
        this.orbit = orbit;
        this.radius = radius;
        this.rotationSpeed = rotationSpeed;
        this.startPos = startPos;
        orbitStart = orbit.position;
    }

    public void MoveProjectile(Rigidbody projectile)
    {
        angle += rotationSpeed * Time.deltaTime*30;
        //Vector3 v = orbit.transform.position + new Vector3(startPos.x * Mathf.Cos(angle) - startPos.y * Mathf.Sin(angle), startPos.x * Mathf.Sin(angle) + startPos.y * Mathf.Cos(angle)) * radius;
        Vector3 v = Quaternion.Euler(0, 0, angle) * (startPos - orbitStart).normalized * radius + orbit.position;
        projectile.MovePosition(v);
    }
}

public class BoomerangProjectileBehavior : IProjectileBehaviour
{
    private Transform origin;
    private Vector3 direction;
    private float duration;
    private float range;
    AnimationCurve curve;

    float time = 0;
    Vector3 eulerAngleVelocity = new Vector3(0, 0, 360);

    public BoomerangProjectileBehavior(Transform origin, Vector3 direction, float duration, float range, AnimationCurve curve)
    {
        this.origin = origin;
        this.direction = direction;
        this.duration = duration;
        this.range = range;
        this.curve = curve;
    }

    public void MoveProjectile(Rigidbody projectile)
    {
        time += Time.deltaTime / duration;
        projectile.MovePosition(origin.position + direction * range * curve.Evaluate(time));
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * 2 * Time.fixedDeltaTime);
        projectile.MoveRotation(projectile.rotation * deltaRotation);
    }
}
