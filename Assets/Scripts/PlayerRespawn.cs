using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Required to restart the level

public class PlayerRespawn : MonoBehaviour
{
    [Header("Health System")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI References")]
    public GameObject deathScreen; // Drag your UI Panel here

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

    private PlayerRespawn playerHealth;

    void Start()
    {
        playerHealth = GetComponentInParent<PlayerRespawn>();
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.currentHealth <= 0)
        {
            return;
        }

    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. Show the UI
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
            Debug.Log("Death Screen Activated!");
        }

        // 2. Hide Visuals
        GetComponent<SpriteRenderer>().enabled = false;

        // 3. Disable Gun/Movement Scripts specifically
        // Replace "PlayerMovement" and "GunScript" with your actual script names
        if (GetComponent<PlayerMovement>() != null) GetComponent<PlayerMovement>().enabled = false;

        // If the gun is a child object:
        Transform gun = transform.Find("PlayerShooting");
        if (gun != null) gun.gameObject.SetActive(false);

        // 4. Stop Physics
        rb.velocity = Vector2.zero;
        rb.isKinematic = true; // Prevents enemies from pushing your "corpse"
    }

    // Call this from your "Start Again" UI Button
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}