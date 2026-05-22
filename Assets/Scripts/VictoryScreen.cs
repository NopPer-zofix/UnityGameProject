using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class VictoryScreen : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("The exact name of your Main Menu scene in Build Settings.")]
    [SerializeField] private string menuSceneName = "MainMenu";

    private void OnEnable()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    
    public void ReturnToMainMenu()
    {
        if (!string.IsNullOrEmpty(menuSceneName))
        {
            Debug.Log($"[Victory Screen] Returning to menu: {menuSceneName}");
            SceneManager.LoadScene(menuSceneName);
        }
        else
        {
            Debug.LogError("[Victory Screen] Menu Scene Name is missing in the Inspector!");
        }
    }
}