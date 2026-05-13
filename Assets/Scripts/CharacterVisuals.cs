using UnityEngine;

public class CharacterVisuals : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer playerSprite;

    void Update()
    {
        // 1. Get Mouse Position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // 2. Check if mouse is on the left of the player
        bool isLeft = mousePos.x < transform.position.x;

        // 3. Send the signal to the Animator
        // This sets the "isFacingLeft" parameter we will create in Step 2
        animator.SetBool("isFacingLeft", isLeft);

        // 4. IMPORTANT: Keep flipX OFF
        // Since you have separate animations for the left, we don't want 
        // the engine to mirror them, or your 'Left' animation will look 'Right' again.
        playerSprite.flipX = false; 
    }
}