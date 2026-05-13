using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Animator animator;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        // Get the Rigidbody2D component attached to the player
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. Get raw values
    float x = Input.GetAxisRaw("Horizontal");
    float y = Input.GetAxisRaw("Vertical");

    moveInput = new Vector2(x, y);

    // 2. Set the Animator value BEFORE normalizing 
    // We use magnitude so it's always a positive number (0 to 1)
    if (animator != null)
    {
        animator.SetFloat("speed", moveInput.magnitude);
    }

    // 3. Now normalize for movement
    if (moveInput.sqrMagnitude > 0.01f)
    {
        moveInput.Normalize();
    }

    }

    void FixedUpdate()
    {
        // 3. Apply velocity to the Rigidbody
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}