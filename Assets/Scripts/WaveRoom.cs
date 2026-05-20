using UnityEngine;

public class WaveRoom : MonoBehaviour
{
    [Header("Room Enemies")]
    public EnemyAI[] roomEnemies;

    [Header("Optional")]
    [Tooltip("A wall/gate to block the entrance after the player enters.")]
    public GameObject door;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip aggroSound; // Drag your aggressive zombie roar clip here
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;

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

        // PLAY AGGRO/AMBUSH SOUND
        if (aggroSound != null)
        {
            // If you want the sound to feel like it comes from the wall, use door.transform.position
            // Otherwise, transform.position uses the center of this trigger room zone
            Vector3 soundPosition = door != null ? door.transform.position : transform.position;
            AudioSource.PlayClipAtPoint(aggroSound, soundPosition, volume);
        }

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