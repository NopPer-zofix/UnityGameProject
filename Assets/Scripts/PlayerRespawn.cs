using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// PlayerRespawn — attach to the Player GameObject.
///
/// Fix: the respawn coroutine runs on RespawnManager (always active),
/// so it never hits the "can't start coroutine on inactive object" error.
///
/// Visual death is handled by disabling the SpriteRenderer + Collider
/// instead of the whole GameObject, keeping the MonoBehaviour alive.
/// </summary>
public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn")]
    [Tooltip("Leave empty to respawn at the position this object starts at.")]
    public Transform spawnPoint;

    [Tooltip("Seconds between death and reappearance.")]
    public float respawnDelay = 1.5f;

    // Callbacks — subscribe from other systems (UI, score, audio, etc.)
    public event Action OnPlayerDied;
    public event Action OnPlayerSpawned;

    // ── private ──────────────────────────────────────────────────────────────
    private Vector3 defaultSpawn;
    private bool isDead = false;

    // Components we hide instead of deactivating the whole GameObject
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private Rigidbody2D rb;

    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        defaultSpawn   = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        col            = GetComponent<Collider2D>();
        rb             = GetComponent<Rigidbody2D>();
    }

    // ─────────────────────────────────────────────────────────────────────────
    /// <summary>
    /// Called by EnemyAI (or any damage source) to kill the player.
    /// </summary>
    public void Die(Action onComplete = null)
    {
        if (isDead) return;
        isDead = true;

        OnPlayerDied?.Invoke();

        // Hide visuals and disable physics — but keep the GameObject active
        // so coroutines and component references stay valid.
        SetVisible(false);

        // Hand the coroutine off to the always-alive RespawnManager.
        RespawnManager.Instance.RunRespawn(RespawnRoutine(onComplete));
    }

    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator RespawnRoutine(Action onComplete)
    {
        yield return new WaitForSeconds(respawnDelay);

        // Teleport to spawn point
        Vector3 target = spawnPoint != null ? spawnPoint.position : defaultSpawn;
        transform.position = target;

        // Reset physics
        if (rb != null) { rb.velocity = Vector2.zero; rb.angularVelocity = 0f; }

        // Restore visuals
        SetVisible(true);

        isDead = false;
        OnPlayerSpawned?.Invoke();
        onComplete?.Invoke();
    }

    // ─────────────────────────────────────────────────────────────────────────
    void SetVisible(bool visible)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = visible;
        if (col            != null) col.enabled            = visible;
    }

    // ─────────────────────────────────────────────────────────────────────────
    void OnDrawGizmos()
    {
        Vector3 pos = (spawnPoint != null) ? spawnPoint.position : defaultSpawn;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, 0.3f);
        Gizmos.DrawLine(pos + Vector3.up    * 0.3f, pos - Vector3.up    * 0.3f);
        Gizmos.DrawLine(pos + Vector3.right * 0.3f, pos - Vector3.right * 0.3f);
    }
}
