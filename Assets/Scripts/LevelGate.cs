using UnityEngine;

public class LevelGate : MonoBehaviour
{
    [Header("Next Level Settings")]
    [SerializeField] private string nextSceneName;

    [Header("UI System Links")]
    [SerializeField] private GameObject lockedMessageUI; 
    [SerializeField] private DialogueManager dialogueManager; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null && inventory.HasKey)
            {
                inventory.UseKey();
                
                // Turn off the locked message if it's currently on screen
                if (lockedMessageUI != null) lockedMessageUI.SetActive(false);
                
                TriggerDialogueSequence();
            }
            else
            {
                ShowLockedMessage();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (lockedMessageUI != null) lockedMessageUI.SetActive(false);
        }
    }

    private void TriggerDialogueSequence()
    {
        if (dialogueManager != null)
        {
            // Turn off player movement here if you want them frozen during dialogue!
            dialogueManager.StartDialogue(nextSceneName);
        }
        else
        {
            // Safety backup: skip straight to scene transition if manager isn't assigned
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }

    private void ShowLockedMessage()
    {
        if (lockedMessageUI != null) lockedMessageUI.SetActive(true);
    }
}