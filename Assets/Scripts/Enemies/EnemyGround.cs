using UnityEngine;

// Inimigo terrestre com máquina de estados: Patrulha → Perseguição → Ataque.
// Nunca sai da própria plataforma: vira (ou para) quando não há chão à frente.
public class EnemyGround : EnemyBase
{
    private enum State { Patrol, Chase, Attack }

    [Header("Plataforma")]
    [SerializeField] private Transform edgeCheck;   // à frente dos pés
    [SerializeField] private Transform wallCheck;   // à frente do corpo
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkDistance = 0.3f;

    private State state = State.Patrol;

    private void Update()
    {
        if (IsDead) return;
        animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocity.x));
    }

    private void FixedUpdate()
    {
        if (IsDead) return;

        // O estado é decidido aqui mesmo: o jogador pode sair da visão entre um Update e outro.
        state = !PlayerInSight ? State.Patrol
              : PlayerInAttackRange() ? State.Attack
              : State.Chase;

        switch (state)
        {
            case State.Patrol:
                if (!CanMoveForward()) Face(-FacingDirection);
                Move(data.patrolSpeed);
                break;

            case State.Chase:
                float toPlayer = player.position.x - transform.position.x;
                Face(toPlayer);
                if (Mathf.Abs(toPlayer) < SameColumnDistance) Stop(); // jogador logo acima: espera embaixo
                else if (CanMoveForward()) Move(data.chaseSpeed);
                else Stop(); // espera na borda em vez de cair
                break;

            case State.Attack:
                Stop();
                Face(player.position.x - transform.position.x);
                TryAttack();
                break;
        }
    }

    private bool CanMoveForward()
    {
        bool groundAhead = Physics2D.Raycast(edgeCheck.position, Vector2.down, checkDistance, groundLayer);
        bool wallAhead = Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, checkDistance, groundLayer);
        return groundAhead && !wallAhead;
    }

    private void Move(float speed) => rb.linearVelocity = new Vector2(FacingDirection * speed, rb.linearVelocity.y);

    private void Stop() => rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.yellow;
        if (edgeCheck != null) Gizmos.DrawLine(edgeCheck.position, edgeCheck.position + Vector3.down * checkDistance);
        if (wallCheck != null) Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * checkDistance * Mathf.Sign(transform.localScale.x));
    }
}
