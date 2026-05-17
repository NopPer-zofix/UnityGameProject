using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Health System")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Visual Effects (No Material Needed)")]
    public float flashDuration = 0.1f;   // How long it stays tinted
    public int flashCount = 3;           // How many times it blinks per hit

    [Header("UI References")]
    public GameObject deathScreen;

    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private Rigidbody2D rb;

    void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        
        if (deathScreen != null) deathScreen.SetActive(false);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Trigger the Tint Flash
            StartCoroutine(FlashRoutine());
        }
    }

    // METHOD 1: Direct color manipulation
    IEnumerator FlashRoutine()
    {
        if (spriteRenderer == null) yield break;

        for (int i = 0; i < flashCount; i++)
        {
            // Turn the sprite red and half-transparent
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f); 
            yield return new WaitForSeconds(flashDuration);

            // Turn it back to normal (White means 100% natural sprite colors)
            spriteRenderer.color = Color.white; 
            yield return new WaitForSeconds(flashDuration);
        }
    }

    public void Die()
{
    if (isDead) return;
    isDead = true;
    
    StopAllCoroutines();
    
    // STOP FOOTSTEPS ON DEATH
    AudioSource audio = GetComponent<AudioSource>();
    if (audio != null) audio.Stop();

    if (spriteRenderer != null) spriteRenderer.enabled = false;
    if (col != null) col.enabled = false;
    if (rb != null) rb.velocity = Vector2.zero;

    if (deathScreen != null) deathScreen.SetActive(true);
}
}