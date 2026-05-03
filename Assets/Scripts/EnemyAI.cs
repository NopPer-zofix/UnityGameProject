using UnityEngine;
using Pathfinding;   // A* Pathfinding Project namespace

/// <summary>
/// EnemyAI — Hotline Miami-style enemy with full A* pathfinding.
///
/// Requires: A* Pathfinding Project (free) imported into the project.
/// https://arongranberg.com/astar
///
/// The enemy recalculates a path to the player every repathRate seconds,
/// then follows the waypoints one by one using MovePosition so walls
/// are respected by physics as well.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed        = 3f;
    public float stoppingDistance = 1.2f;   // melee / kill range

    [Header("Pathfinding")]
    [Tooltip("How often (seconds) to recalculate the path to the player.")]
    public float repathRate      = 0.5f;
    [Tooltip("How close the enemy must get to a waypoint before moving to the next.")]
    public float waypointReached = 0.4f;

    [Header("References")]
    public Transform player;

    // ── private ──────────────────────────────────────────────────────────────
    private Rigidbody2D   rb;
    private PlayerRespawn playerRespawn;
    private bool          playerDead = false;

    // A* path data
    private Path    currentPath;
    private int     waypointIndex;
    private float   repathTimer;
    private bool    pathPending = false;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation          = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation         = true;

        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null)
            {
                player        = obj.transform;
                playerRespawn = obj.GetComponent<PlayerRespawn>();
            }
        }
        else
        {
            playerRespawn = player.GetComponent<PlayerRespawn>();
        }

        // Request the first path immediately
        RequestPath();
    }

    // ─────────────────────────────────────────────────────────────────────────
    void FixedUpdate()
    {
        if (player == null || playerDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Recalculate path periodically so the enemy reacts to player movement
        repathTimer += Time.fixedDeltaTime;
        if (repathTimer >= repathRate)
        {
            repathTimer = 0f;
            RequestPath();
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= stoppingDistance)
        {
            rb.velocity = Vector2.zero;
            KillPlayer();
            return;
        }

        FollowPath();
    }

    // ─────────────────────────────────────────────────────────────────────────
    /// Ask A* to calculate a path from enemy position to player position.
    void RequestPath()
    {
        if (player == null || pathPending) return;
        pathPending = true;
        ABPath path = ABPath.Construct(transform.position, player.position, OnPathComplete);
        AstarPath.StartPath(path);
    }

    // ─────────────────────────────────────────────────────────────────────────
    /// Callback fired by A* when the path is ready.
    void OnPathComplete(Path p)
    {
        pathPending = false;

        if (p.error)
        {
            Debug.LogWarning("EnemyAI: path error — " + p.errorLog);
            return;
        }

        currentPath    = p;
        waypointIndex  = 0;   // start from the beginning of the new path
    }

    // ─────────────────────────────────────────────────────────────────────────
    /// Move along the current path waypoint by waypoint.
    void FollowPath()
    {
        if (currentPath == null) return;
        if (waypointIndex >= currentPath.vectorPath.Count) return;

        Vector2 target = currentPath.vectorPath[waypointIndex];
        Vector2 dir    = (target - rb.position).normalized;

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        rb.velocity = Vector2.zero;

        Flip(dir.x);

        // Advance to next waypoint once close enough
        if (Vector2.Distance(rb.position, target) <= waypointReached)
            waypointIndex++;
    }

    // ─────────────────────────────────────────────────────────────────────────
    void KillPlayer()
    {
        if (playerRespawn == null) return;
        playerDead = true;
        playerRespawn.Die(OnRespawnComplete);
    }

    void OnRespawnComplete() => playerDead = false;

    // ─────────────────────────────────────────────────────────────────────────
    void Flip(float horizontalDir)
    {
        if      (horizontalDir >  0.1f) transform.localScale = new Vector3( 1, 1, 1);
        else if (horizontalDir < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
    }

    // ─────────────────────────────────────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        // Draw the current path so you can see it in the Scene view
        if (currentPath == null) return;
        Gizmos.color = Color.yellow;
        for (int i = waypointIndex; i < currentPath.vectorPath.Count - 1; i++)
            Gizmos.DrawLine(currentPath.vectorPath[i], currentPath.vectorPath[i + 1]);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}