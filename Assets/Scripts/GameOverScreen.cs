using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Text")]
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI bestScoreTxt;

    [Header("Buttons")]
    public Button restartBtn;
    public Button menuBtn;

    [Header("HUD")]
    public GameObject hud; // drag your HUD Canvas/object here

    public bool IsShowing { get; private set; } = false;

    void Start()
    {
        panel.SetActive(false);
        restartBtn.onClick.AddListener(Restart);
        menuBtn.onClick.AddListener(GoToMenu);
    }

    public void Show(int score)
    {
        IsShowing = true;
        Time.timeScale = 0f;
        panel.SetActive(true);

        if (hud != null) hud.SetActive(false); // hide HUD on death
    }

    void Restart()
    {
        IsShowing = false;
        Time.timeScale = 1f;
        AudioManager.Instance?.PlayLevelMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GoToMenu()
    {
        IsShowing = false;
        Time.timeScale = 1f;
        AudioManager.Instance?.PlayMenuMusic();
        SceneManager.LoadScene("menu");
    }
}