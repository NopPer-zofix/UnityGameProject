using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Animator animator;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Vector2 moveInput;

    void Start()
    {
        // Get the Rigidbody2D component attached to the player
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (PauseMenu.IsPaused)
        {
            moveInput = Vector2.zero;
            // Stop looping footstep sound on pause
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
            if (animator != null)
                animator.SetFloat("speed", 0f);
            return;
        }

        // 1. Get raw values
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(x, y);

        // 2. Set the Animator value BEFORE normalizing 
        if (animator != null)
        {
            animator.SetFloat("speed", moveInput.magnitude);
        }

        // 3. Simple Audio Play/Stop
        if (audioSource != null)
        {
            if (moveInput.sqrMagnitude > 0.01f)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }

        // 4. Now normalize for movement
        if (moveInput.sqrMagnitude > 0.01f)
        {
            moveInput.Normalize();
        }
    }

    void FixedUpdate()
    {
        if (PauseMenu.IsPaused) { rb.velocity = Vector2.zero; return; }
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}