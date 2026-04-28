using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        // Get the Rigidbody2D component attached to the player
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. Get Input (Horizontal = A/D or Left/Right, Vertical = W/S or Up/Down)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // 2. Normalize diagonal movement so the player doesn't move faster diagonally
        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        // 3. Apply velocity to the Rigidbody
        rb.velocity = moveInput * moveSpeed;
    }
}