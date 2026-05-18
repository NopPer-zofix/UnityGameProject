// using UnityEngine;
//
// public class EnemyHealth : MonoBehaviour
// {
//     public int health = 3;
//
//     [Tooltip("The room this enemy belongs to. Drag the WaveRoom trigger here.")]
//     public WaveRoom room;
//
//     // ─────────────────────────────────────────────────────────────────────────
//     public void TakeDamage(int amount)
//     {
//         health -= amount;
//
//         if (health <= 0)
//             Die();
//     }
//
//     // ─────────────────────────────────────────────────────────────────────────
//     void Die()
//     {
//         Debug.Log("Enemy is dead!");
//
//         // Notify WaveManager so it can check if the room / level is cleared
//         if (WaveManager.Instance != null)
//             WaveManager.Instance.OnEnemyDied(room);
//
//         Destroy(gameObject);
//     }
// }

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
        if (health <= 0) return; // already dying, ignore extra hits
        health -= amount;
        if (health <= 0)
            StartCoroutine(DieRoutine());
    }

    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator DieRoutine()
    {
        // Stop AI so enemy freezes in place during death animation
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        // Stop physics
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        // Disable collider so bullets pass through the corpse
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Play death animation
        if (animator != null)
            animator.SetTrigger("death");

        // Wait for animation to finish
        yield return new WaitForSeconds(deathAnimDuration);

        // Notify WaveManager then destroy
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemyDied(room);

        Debug.Log("Enemy is dead!");
        Destroy(gameObject);
    }
}