using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Panel")]
    public GameObject pausePanel;

    [Header("Buttons")]
    public Button continueBtn;
    public Button settingsBtn;
    public Button quitLevelBtn;

    [Header("Settings Sub-Panel (optional)")]
    public GameObject settingsPanel;
    public Button settingsBackBtn;
    public Slider musicSlider;
    public Slider sfxSlider;

    bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);

        continueBtn.onClick.AddListener(Resume);
        settingsBtn.onClick.AddListener(OpenSettings);
        quitLevelBtn.onClick.AddListener(QuitLevel);

        if (settingsBackBtn) settingsBackBtn.onClick.AddListener(CloseSettings);

        // Hook sliders if settings panel exists in pause menu
        if (musicSlider)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 1f);
            musicSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetMusicVolume(v));
        }
        if (sfxSlider)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 1f);
            sfxSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetSFXVolume(v));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.Instance?.Dim(); // quietly duck the music
    }

    void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        AudioManager.Instance?.Undim();
    }

    void OpenSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    void CloseSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    void QuitLevel()
    {
        // Restore time and music before leaving
        Time.timeScale = 1f;
        AudioManager.Instance?.PlayMenuMusic();
        SceneManager.LoadScene("menu");
    }
}
