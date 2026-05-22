using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueLine
    {
        public Sprite characterSprite; 
        [TextArea(3, 10)]
        public string text;            
    }

    [Header("UI Fields")]
    [SerializeField] private TextMeshProUGUI textDisplay;
    
    [Header("Dialogue Content")]
    public DialogueLine[] dialogueLines; 
    
    private int currentLineIndex = 0;
    private string targetSceneName;
    private PlayerShooting playerShooting; 

    public void StartDialogue(string nextScene)
    {
        targetSceneName = nextScene;
        gameObject.SetActive(true); 
        currentLineIndex = 0;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerShooting = player.GetComponentInChildren<PlayerShooting>();
            if (playerShooting != null)
            {
                playerShooting.CanShoot = false; // LOCK GUNS
            }
        }
        
        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        if (dialogueLines.Length > 0 && currentLineIndex < dialogueLines.Length)
        {
            DialogueLine currentLine = dialogueLines[currentLineIndex];
            textDisplay.text = currentLine.text;

            
        }
    }

    public void AdvanceDialogue()
    {
        currentLineIndex++;

        if (currentLineIndex < dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        if (playerShooting != null)
        {
            playerShooting.CanShoot = true; 
        }

        gameObject.SetActive(false);
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}