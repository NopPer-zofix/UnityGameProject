using System.Collections;
using UnityEngine;

/// <summary>
/// RespawnManager — a persistent, always-active singleton.
///
/// Unity can't run coroutines on inactive GameObjects. This manager
/// stays alive and runs the respawn coroutine on behalf of the Player,
/// which may be visually hidden during the death delay.
///
/// Setup: Create an empty GameObject in your scene, name it
/// "RespawnManager", and attach this script. That's it — one per scene.
/// </summary>
public class RespawnManager : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static RespawnManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);    // only one allowed
            return;
        }
        Instance = this;
        // Optional: uncomment the line below if you want it to survive
        // scene loads (e.g. for a persistent game manager pattern).
        // DontDestroyOnLoad(gameObject);
    }

    // ─────────────────────────────────────────────────────────────────────────
    /// <summary>
    /// Runs an IEnumerator coroutine on this always-active GameObject.
    /// Called by PlayerRespawn.Die() so the coroutine keeps running
    /// even while the Player object is visually inactive.
    /// </summary>
    public void RunRespawn(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}