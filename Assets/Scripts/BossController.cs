using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{
    [Header("Boss Stats")]
    public int maxHealth = 100;
    public int CurrentHealth => currentHealth;

    [Header("Phase Positions")]
    public Transform phase1Position;
    public Transform phase2Position;
    public Transform phase3Position;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Phase 1 — Small enemies")]
    public GameObject enemyType1Prefab;
    public int        phase1SpawnCount  = 12;   // 12 at a time
    public int        phase1DamagePerKill = 1;  // 1 kill = 1 hp

    [Header("Phase 2 — Medium enemies")]
    public GameObject enemyType2Prefab;
    public int        phase2SpawnCount  = 6;    // 6 at a time
    public int        phase2DamagePerKill = 3;  // 1 kill = 3 hp

    [Header("Phase 3 — Large enemies")]
    public GameObject enemyType3Prefab;
    public int        phase3SpawnCount  = 4;    // 4 at a time
    public int        phase3DamagePerKill = 5;  // 1 kill = 5 hp

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public float     spawnRadius = 4f;
    public float     waveDelay  = 1f;

    [Header("Boss Model Parts")]
    public GameObject[] smallHeads;
    public GameObject[] mediumHeads;

    [Header("Win")]
    public GameObject winScreen;

    [Header("Animation")]
    public Animator animator;
    [Tooltip("Must match spawn animation length in seconds — enemies spawn after this.")]
    public float spawnAnimDuration = 1f;
    [Tooltip("Must match death animation length in seconds.")]
    public float deathAnimDuration = 1.5f;

    // ── private ──────────────────────────────────────────────────────────────
    private int  currentHealth;
    private int  currentPhase = 0;
    private bool isDead       = false;
    private bool inTransition = false;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private Coroutine waveLoopCoroutine;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        currentHealth = maxHealth;
        StartCoroutine(MoveTo(phase1Position, () => {
            currentPhase = 1;
            waveLoopCoroutine = StartCoroutine(WaveLoop());
        }));
    }

    // ─────────────────────────────────────────────────────────────────────────
    public void OnEnemyKilled(GameObject enemy, int damageAmount)
    {
        if (isDead || inTransition) return;

        activeEnemies.Remove(enemy);
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);
        Debug.Log($"[Boss] HP: {currentHealth}/{maxHealth} | Remaining in wave: {activeEnemies.Count}");

        float pct = (float)currentHealth / maxHealth;

        if      (currentPhase == 1 && pct <= 0.76f) StartCoroutine(EnterPhase(2));
        else if (currentPhase == 2 && pct <= 0.4f) StartCoroutine(EnterPhase(3));
        else if (currentPhase == 3 && currentHealth <= 0) StartCoroutine(Die());
    }

    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator WaveLoop()
    {
        while (!isDead && !inTransition)
        {
            yield return StartCoroutine(SpawnWave());

            yield return new WaitUntil(() => activeEnemies.Count == 0 || isDead || inTransition);

            if (isDead || inTransition) yield break;

            yield return new WaitForSeconds(waveDelay);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator SpawnWave()
    {
        // Play spawn animation and wait for it to finish
        if (animator != null)
            animator.SetTrigger("spawn");

        yield return new WaitForSeconds(spawnAnimDuration);

        activeEnemies.RemoveAll(e => e == null);

        GameObject prefab      = currentPhase == 1 ? enemyType1Prefab :
                                 currentPhase == 2 ? enemyType2Prefab :
                                                     enemyType3Prefab;

        int spawnCount         = currentPhase == 1 ? phase1SpawnCount :
                                 currentPhase == 2 ? phase2SpawnCount :
                                                     phase3SpawnCount;

        int damagePerKill      = currentPhase == 1 ? phase1DamagePerKill :
                                 currentPhase == 2 ? phase2DamagePerKill :
                                                     phase3DamagePerKill;

        if (prefab == null || spawnPoint == null) yield break;

        for (int i = 0; i < spawnCount; i++)
        {
            // Spread enemies evenly in a circle
            float   angle    = i * (360f / spawnCount) * Mathf.Deg2Rad;
            Vector2 offset   = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
            Vector3 spawnPos = spawnPoint.position + new Vector3(offset.x, offset.y, 0);

            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

            BossEnemyHealth beh = enemy.GetComponent<BossEnemyHealth>();
            if (beh != null)
            {
                beh.boss            = this;
                beh.damageOnDeath   = damagePerKill;
            }

            activeEnemies.Add(enemy);
        }

        Debug.Log($"[Boss] Spawned {spawnCount} enemies (phase {currentPhase}, {damagePerKill} dmg/kill)");
    }

    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator EnterPhase(int phase)
    {
        inTransition = true;

        if (waveLoopCoroutine != null)
        {
            StopCoroutine(waveLoopCoroutine);
            waveLoopCoroutine = null;
        }

        foreach (GameObject e in activeEnemies)
            if (e != null) Destroy(e);
        activeEnemies.Clear();

        if (phase == 2)
            foreach (GameObject h in smallHeads)
                if (h != null) h.SetActive(false);

        if (phase == 3)
            foreach (GameObject h in mediumHeads)
                if (h != null) h.SetActive(false);

        Transform target = phase == 2 ? phase2Position : phase3Position;
        yield return StartCoroutine(MoveTo(target));

        currentPhase = phase;
        inTransition = false;

        waveLoopCoroutine = StartCoroutine(WaveLoop());
    }

    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator MoveTo(Transform target, System.Action onArrival = null)
    {
        if (target == null) { onArrival?.Invoke(); yield break; }

        // Start walking animation
        if (animator != null)
            animator.SetFloat("speed", 1f);

        while (Vector2.Distance(transform.position, target.position) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target.position;

        // Stop walking animation
        if (animator != null)
            animator.SetFloat("speed", 0f);

        // Rescan so enemies recalculate paths around the boss new position
        if (AstarPath.active != null)
            AstarPath.active.Scan();

        onArrival?.Invoke();
    }

    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator Die()
    {
        isDead = true;

        if (waveLoopCoroutine != null)
            StopCoroutine(waveLoopCoroutine);

        foreach (GameObject e in activeEnemies)
            if (e != null) Destroy(e);
        activeEnemies.Clear();

        Debug.Log("[Boss] Defeated! Game Won.");

        if (animator != null)
            animator.SetTrigger("death");

        yield return new WaitForSeconds(deathAnimDuration);

        if (winScreen != null) winScreen.SetActive(true);
        Destroy(gameObject);
    }

    // ─────────────────────────────────────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        if (spawnPoint == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(spawnPoint.position, spawnRadius);
    }
}