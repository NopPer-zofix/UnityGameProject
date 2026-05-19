using UnityEngine;
using Pathfinding;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Combat")]
    public float attackRange        = 2f;
    public float damageValue        = 25f;
    public float attackRate         = 1.0f;
    [Tooltip("Must match your attack animation clip length in seconds.")]
    public float attackAnimDuration = 0.8f;

    [Header("Pathfinding")]
    public float repathRate      = 0.5f;
    public float waypointReached = 0.4f;

    [Header("Animation")]
    public Animator animator;

    [Header("References")]
    public Transform player;

    // ── private ──────────────────────────────────────────────────────────────
    private Rigidbody2D   rb;
    private PlayerRespawn playerRespawn;

    private Path  currentPath;
    private int   waypointIndex;
    private float repathTimer;
    private bool  pathPending = false;
    private Vector3 originalScale;

    private float nextAttackTime = 0f;
    private bool  isAttacking   = false;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation          = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation         = true;

        originalScale = transform.localScale;

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

        RequestPath();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Attack detection in Update — runs every frame, never misses
    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange && !isAttacking && Time.time >= nextAttackTime)
            StartCoroutine(AttackRoutine());
    }

    // ─────────────────────────────────────────────────────────────────────────
    void FixedUpdate()
    {
        if (player == null || (playerRespawn != null && playerRespawn.currentHealth <= 0))
        {
            rb.velocity = Vector2.zero;
            SetSpeed(0f);
            return;
        }

        repathTimer += Time.fixedDeltaTime;
        if (repathTimer >= repathRate)
        {
            repathTimer = 0f;
            RequestPath();
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange || isAttacking)
        {
            rb.velocity = Vector2.zero;
            SetSpeed(0f);
        }
        else
        {
            FollowPath();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // Fire the trigger — guaranteed to be picked up this frame
        if (animator != null)
            animator.SetTrigger("attack");

        // Wait for animation to finish
        yield return new WaitForSeconds(attackAnimDuration);

        // Check damage at last frame
        if (player != null && playerRespawn != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= attackRange)
                playerRespawn.TakeDamage(damageValue);
        }

        nextAttackTime = Time.time + attackRate;
        isAttacking    = false;
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
        if (!p.error) { currentPath = p; waypointIndex = 0; }
    }

    // ─────────────────────────────────────────────────────────────────────────
    void FollowPath()
    {
        if (currentPath == null || waypointIndex >= currentPath.vectorPath.Count)
        {
            SetSpeed(0f);
            return;
        }

        Vector2 target = currentPath.vectorPath[waypointIndex];
        Vector2 dir    = (target - (Vector2)transform.position).normalized;

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        rb.velocity = Vector2.zero;

        SetSpeed(1f);
        Flip(dir.x);

        if (Vector2.Distance(transform.position, target) <= waypointReached)
            waypointIndex++;
    }

    // ─────────────────────────────────────────────────────────────────────────
    void SetSpeed(float speed)
    {
        if (animator != null)
            animator.SetFloat("speed", speed);
    }

    void Flip(float horizontalDir)
    {
        if (horizontalDir > 0.1f)
            transform.localScale = new Vector3( Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (horizontalDir < -0.1f)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    // ─────────────────────────────────────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (currentPath == null) return;
        Gizmos.color = Color.yellow;
        for (int i = waypointIndex; i < currentPath.vectorPath.Count - 1; i++)
            Gizmos.DrawLine(currentPath.vectorPath[i], currentPath.vectorPath[i + 1]);
    }
}