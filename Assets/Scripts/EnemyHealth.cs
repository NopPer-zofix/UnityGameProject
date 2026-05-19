using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;

    [Tooltip("The room this enemy belongs to. Drag the WaveRoom trigger here.")]
    public WaveRoom room;

    [Header("Animation")]
    public Animator animator;
    [Tooltip("Must match your death animation clip length in seconds.")]
    public float deathAnimDuration = 1f;

    // ─────────────────────────────────────────────────────────────────────────
    public void TakeDamage(int amount)
    {
        if (health <= 0) return;
        health -= amount;
        if (health <= 0)
            StartCoroutine(DieRoutine());
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Virtual so BossEnemyHealth can override it
    protected virtual IEnumerator DieRoutine()
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

        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemyDied(room);

        Debug.Log("Enemy is dead!");
        Destroy(gameObject);
    }
}