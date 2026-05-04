using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;

    [Tooltip("The room this enemy belongs to. Drag the WaveRoom trigger here.")]
    public WaveRoom room;

    // ─────────────────────────────────────────────────────────────────────────
    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
            Die();
    }

    // ─────────────────────────────────────────────────────────────────────────
    void Die()
    {
        Debug.Log("Enemy is dead!");

        // Notify WaveManager so it can check if the room / level is cleared
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemyDied(room);

        Destroy(gameObject);
    }
}