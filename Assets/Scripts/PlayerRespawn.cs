using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Health System")]
    public float maxHealth = 100f;
    public float currentHealth;
    public HeartsUI heartsUI;       // drag HeartsUI object here

    [Header("Visual Effects (No Material Needed)")]
    public float flashDuration = 0.1f;   
    public int flashCount = 3;           

    [Header("Death References")]
    [SerializeField] private Animator animator;
    public GameObject deathScreen;
    [SerializeField] private float deathAnimationDelay = 1.5f; 

    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private Rigidbody2D rb;
    private PlayerMovement playerMovementScript; 
    private PlayerShooting shootingScript;
    private GameObject gunPivotObject; // Automatically tracks your GunPivot

    void Start()
    {
        // Initialise hearts to full on level load
        if (heartsUI != null)
            heartsUI.UpdateHearts((int)currentHealth);
    }

    void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        
        playerMovementScript = GetComponent<PlayerMovement>();
        shootingScript = GetComponent<PlayerShooting>();

        // 1. Automatically find the GunPivot child by its exact name
        Transform pivotTransform = transform.Find("GunPivot");
        if (pivotTransform != null)
        {
            gunPivotObject = pivotTransform.gameObject;
        }
        
        if (deathScreen != null) deathScreen.SetActive(false);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (heartsUI != null)
            heartsUI.UpdateHearts((int)currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashRoutine());
        }
    }

    IEnumerator FlashRoutine()
    {
        if (spriteRenderer == null) yield break;

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f); 
            yield return new WaitForSeconds(flashDuration);

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
        
        if (shootingScript != null)
        {
            shootingScript.enabled = false; 
        }

        // 2. Instantly disable the GunPivot container and everything inside it
        if (gunPivotObject != null)
        {
            gunPivotObject.SetActive(false);
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (col != null) col.enabled = false;
        if (rb != null) rb.velocity = Vector2.zero;

        StartCoroutine(PlayerDeathSequence());
    }

    private IEnumerator PlayerDeathSequence()
    {
        yield return new WaitForSeconds(deathAnimationDelay);

        if (spriteRenderer != null) spriteRenderer.enabled = false;

        if (deathScreen != null) deathScreen.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}