using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    // Set this in the inspector for each ground item (e.g., Shotgun or Rifle)
    public PlayerShooting.WeaponType weaponToGive; 

    private bool playerIsClose = false;
    private PlayerShooting playerShootingScript;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip pickupSound; // Drag your audio clip here in the Inspector
    

    void Update()
    {
        // If the player is standing near the weapon and presses F
        if (playerIsClose && Input.GetKeyDown(KeyCode.F))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        if (playerShootingScript != null)
        {
            // Tell the player script to change its active child model and stats
            playerShootingScript.EquipWeapon(weaponToGive);
            
            Destroy(gameObject);
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
        }
    }

    // Detect when the player walks into the weapon's physical space
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerShootingScript = collision.GetComponent<PlayerShooting>();
            if (playerShootingScript != null)
            {
                playerIsClose = true;
                Debug.Log("Press 'F' to pick up " + weaponToGive);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            playerShootingScript = null;
        }
    }
}