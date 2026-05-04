using UnityEngine;
using System.Collections;

/// <summary>
/// WaveManager — one per level, tracks room clears.
///
/// Setup:
///   1. Empty GameObject in the scene named "WaveManager".
///   2. Attach this script.
///   3. Drag all WaveRoom objects into "Rooms".
///
/// EnemyHealth calls WaveManager.Instance.OnEnemyDied(myRoom)
/// whenever an enemy dies. When a whole room is cleared, that room
/// is counted. When all rooms are cleared → OnLevelComplete fires.
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("All rooms in this level")]
    public WaveRoom[] rooms;

    [Tooltip("Delay after last enemy dies before level-complete triggers.")]
    public float levelCompleteDelay = 2f;

    private int roomsCleared = 0;
    private bool levelDone   = false;

    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ─────────────────────────────────────────────────────────────────────────
    /// Called by EnemyHealth when an enemy dies.
    /// <param name="room">The WaveRoom that enemy belonged to.</param>
    public void OnEnemyDied(WaveRoom room)
    {
        if (levelDone || room == null) return;

        if (room.IsCleared())
        {
            roomsCleared++;
            Debug.Log($"[WaveManager] Room cleared! {roomsCleared}/{rooms.Length}");

            if (roomsCleared >= rooms.Length)
            {
                levelDone = true;
                StartCoroutine(LevelCompleteRoutine());
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator LevelCompleteRoutine()
    {
        Debug.Log("[WaveManager] All rooms cleared — level complete!");
        yield return new WaitForSeconds(levelCompleteDelay);

        // Hook your card upgrade screen here, e.g.:
        // CardUpgradeUI.Instance.Show();
        // SceneManager.LoadScene("CardUpgradeScreen");
    }
}
