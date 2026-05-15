using UnityEngine;
using Pathfinding;   // A* Pathfinding Project namespace

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 1.2f;   // melee / damage range

    [Header("Pathfinding")]
    public float repathRate = 0.5f;
    public float waypointReached = 0.4f;

    [Header("Combat")]
    public float damageValue = 25f;
    public float attackRate = 1.0f;
    private float nextAttackTime;

    [Header("References")]
    public Transform player;

    // ── private ──────────────────────────────────────────────────────────────
    private Rigidbody2D rb;
    private PlayerRespawn playerRespawn;

    private Path currentPath;
    private int waypointIndex;
    private float repathTimer;
    private bool pathPending = false;
    private Vector3 originalScale;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;

        originalScale = transform.localScale;

        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null)
            {
                player = obj.transform;
                playerRespawn = obj.GetComponent<PlayerRespawn>();
            }
        }
        else
        {
            playerRespawn = player.GetComponent<PlayerRespawn>();
        }

        RequestPath();
    }

    // ── MERGED FIXED UPDATE ──────────────────────────────────────────────────
    void FixedUpdate()
    {
        // 1. Safety Check: If player is dead or missing, stop and do nothing.
        if (player == null || (playerRespawn != null && playerRespawn.currentHealth <= 0))
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // 2. A* Timer: Recalculate path periodically
        repathTimer += Time.fixedDeltaTime;
        if (repathTimer >= repathRate)
        {
            repathTimer = 0f;
            RequestPath();
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // 3. Combat Logic: If close enough, stop and attack
        if (distToPlayer <= stoppingDistance)
        {
            rb.velocity = Vector2.zero;

            if (Time.time >= nextAttackTime)
            {
                playerRespawn.TakeDamage(damageValue);
                nextAttackTime = Time.time + attackRate;
            }
            return;
        }

        // 4. Movement: If not attacking, follow the path
        FollowPath();
    }

    // ─────────────────────────────────────────────────────────────────────────
    void RequestPath()
    {
        if (player == null || pathPending) return;
        pathPending = true;
        ABPath path = ABPath.Construct(transform.position, player.position, OnPathComplete);
        AstarPath.StartPath(path);
    }

    void OnPathComplete(Path p)
    {
        pathPending = false;
        if (!p.error)
        {
            currentPath = p;
            waypointIndex = 0;
        }
    }

    void FollowPath()
    {
        if (currentPath == null || waypointIndex >= currentPath.vectorPath.Count) return;

        Vector2 target = currentPath.vectorPath[waypointIndex];
        Vector2 dir = (target - (Vector2)transform.position).normalized;

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        rb.velocity = Vector2.zero;

        Flip(dir.x);

        if (Vector2.Distance(transform.position, target) <= waypointReached)
            waypointIndex++;
    }

    void Flip(float horizontalDir)
    {
        if (horizontalDir > 0.1f)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (horizontalDir < -0.1f)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    void OnDrawGizmosSelected()
    {
        if (currentPath == null) return;
        Gizmos.color = Color.yellow;
        for (int i = waypointIndex; i < currentPath.vectorPath.Count - 1; i++)
            Gizmos.DrawLine(currentPath.vectorPath[i], currentPath.vectorPath[i + 1]);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}