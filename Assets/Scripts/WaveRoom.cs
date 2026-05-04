using UnityEngine;

/// <summary>
/// WaveRoom — attach to an invisible trigger GameObject (the doorway).
///
/// When the player walks in, all enemies in this room have their
/// EnemyAI enabled and start chasing.
///
/// Setup per room:
///   1. Empty GameObject at the doorway entrance.
///   2. BoxCollider2D → Is Trigger = ON.
///   3. Attach this script.
///   4. Drag all enemy GameObjects for this room into "Room Enemies".
///   5. (Optional) drag a door GameObject into "Door" — it activates on entry.
/// </summary>
public class WaveRoom : MonoBehaviour
{
    [Header("Room Enemies")]
    public EnemyAI[] roomEnemies;

    [Header("Optional")]
    [Tooltip("A wall/gate to block the entrance after the player enters.")]
    public GameObject door;

    // ── private ──────────────────────────────────────────────────────────────
    private bool activated = false;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        // All enemies in this room start inactive — they don't chase yet
        foreach (EnemyAI enemy in roomEnemies)
            if (enemy != null)
                enemy.enabled = false;
    }

    // ─────────────────────────────────────────────────────────────────────────
    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;

        // Wake up all enemies
        foreach (EnemyAI enemy in roomEnemies)
            if (enemy != null)
                enemy.enabled = true;

        // Close the door behind the player (optional)
        if (door != null)
            door.SetActive(true);

        // Disable trigger so it only fires once
        GetComponent<Collider2D>().enabled = false;

        Debug.Log($"[WaveRoom] '{gameObject.name}' triggered — {roomEnemies.Length} enemies agro.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    /// Called by EnemyHealth.Die() to check if all enemies in this room are gone.
    public bool IsCleared()
    {
        foreach (EnemyAI enemy in roomEnemies)
            if (enemy != null && enemy.gameObject != null)
                return false;   // Destroy() was called — gameObject becomes null
        return true;
    }
}
