using UnityEngine;
using System.Collections;

public class BossEnemyHealth : EnemyHealth
{
    // Set by BossController.SpawnWave() — how much boss HP this enemy's death removes
    [HideInInspector]
    public BossController boss;
    [HideInInspector]
    public int damageOnDeath = 1;

    // ─────────────────────────────────────────────────────────────────────────
    protected override IEnumerator DieRoutine()
    {
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (animator != null)
            animator.SetTrigger("death");

        yield return new WaitForSeconds(deathAnimDuration);

        // Pass damageOnDeath so boss loses the correct amount of HP
        if (boss != null)
            boss.OnEnemyKilled(gameObject, damageOnDeath);

        Destroy(gameObject);
    }
}