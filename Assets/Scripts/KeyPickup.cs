using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Visual Prompt UI (Optional)")]
    [SerializeField] private GameObject interactPrompt;
    private bool playerInRange = false;
    private PlayerInventory playerInventory;

    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void Update()
    {
        // If player is nearby and presses 'F'
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (playerInventory != null)
            {
                playerInventory.PickUpKey();
                
                if (interactPrompt != null) interactPrompt.SetActive(false);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;
            
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }
}