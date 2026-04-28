using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float stoppingDistance = 2f;
    public Transform player; // Drag the Player here in the Inspector

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // If you forgot to drag the player in, this finds them by Tag
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // 1. Calculate the current gap
        float currentDistance = Vector2.Distance(transform.position, player.position);

        // 2. Decide whether to move or stop
        if (currentDistance > stoppingDistance)
        {
            // Move toward player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
            
            // Flip sprite to face player
            Flip(direction.x);
        }
        else
        {
            // We are within the stopping distance, so stop movement
            rb.velocity = Vector2.zero;
        }
    }

    void Flip(float horizontalDir)
    {
        if (horizontalDir > 0.1f) transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalDir < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
    }
}