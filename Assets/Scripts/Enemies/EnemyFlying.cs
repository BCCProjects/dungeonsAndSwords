using System.Collections.Generic;
using UnityEngine;

// Inimigo voador: persegue o jogador seguindo o caminho do A*, recalculado a cada 0,3–0,5 s.
public class EnemyFlying : EnemyBase
{
    [Header("A*")]
    [SerializeField] private AStarPathfinder pathfinder;
    [SerializeField] private float repathInterval = 0.4f;
    [SerializeField] private float waypointReachDistance = 0.2f;

    [Header("Parado")]
    [SerializeField] private float hoverAmplitude = 0.25f;

    private List<Vector3> path = new List<Vector3>();
    private int pathIndex;
    private float nextRepathTime;
    private Vector3 homePosition;

    private void Start()
    {
        if (pathfinder == null) pathfinder = FindFirstObjectByType<AStarPathfinder>();
        homePosition = transform.position;
    }

    private void Update()
    {
        if (IsDead) return;

        if (PlayerInSight && Time.time >= nextRepathTime)
        {
            nextRepathTime = Time.time + repathInterval;
            path = pathfinder.FindPath(transform.position, PlayerTarget());
            pathIndex = 0;
        }

        animator.SetFloat(SpeedHash, rb.linearVelocity.magnitude);
    }

    private void FixedUpdate()
    {
        if (IsDead) return;

        if (!PlayerInSight)
        {
            // Sem jogador à vista: volta para casa e flutua no lugar.
            Vector3 hover = homePosition + Vector3.up * Mathf.Sin(Time.time * 2f) * hoverAmplitude;
            MoveTowards(hover, data.patrolSpeed);
            return;
        }

        Face(player.position.x - transform.position.x);

        if (PlayerInAttackRange())
        {
            rb.linearVelocity = Vector2.zero;
            TryAttack();
            return;
        }

        if (pathIndex < path.Count)
        {
            MoveTowards(path[pathIndex], data.chaseSpeed);
            if (Vector2.Distance(transform.position, path[pathIndex]) < waypointReachDistance)
                pathIndex++;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Mira no peito do jogador, não nos pés.
    private Vector3 PlayerTarget() => player.position + Vector3.up * 0.8f;

    // No ar a distância conta nos dois eixos.
    protected override bool PlayerInAttackRange() =>
        player != null && Vector2.Distance(transform.position, PlayerTarget()) <= data.attackRange;

    private void MoveTowards(Vector3 target, float speed)
    {
        Vector2 toTarget = target - transform.position;
        rb.linearVelocity = toTarget.magnitude > 0.05f ? toTarget.normalized * speed : Vector2.zero;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.cyan;
        for (int i = pathIndex; i < path.Count - 1; i++)
            Gizmos.DrawLine(path[i], path[i + 1]);
    }
}
