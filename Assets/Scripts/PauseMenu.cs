using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Pause Buttons")]
    public Button continueBtn;
    public Button settingsBtn;
    public Button restartBtn;
    public Button quitLevelBtn;

    [Header("Settings")]
    public Button settingsBackBtn;
    public Slider musicSlider;
    public Slider sfxSlider;

    public static bool IsPaused { get; private set; } = false;

    void Start()
    {
        IsPaused = false;
        pausePanel.SetActive(false);
        settingsPanel?.SetActive(false);

        continueBtn.onClick.AddListener(Resume);
        settingsBtn.onClick.AddListener(OpenSettings);
        restartBtn.onClick.AddListener(Restart);
        quitLevelBtn.onClick.AddListener(QuitLevel);
        settingsBackBtn?.onClick.AddListener(CloseSettings);

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
            if (IsPaused) Resume(); else Pause();
        }
    }

    public void Pause()
    {
        IsPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.Instance?.Dim();
    }

    public void Resume()
    {
        IsPaused = false;
        pausePanel.SetActive(false);
        settingsPanel?.SetActive(false);
        Time.timeScale = 1f;
        AudioManager.Instance?.Undim();
    }

    void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel?.SetActive(true);
    }

    void CloseSettings()
    {
        settingsPanel?.SetActive(false);
        pausePanel.SetActive(true);
    }

    void Restart()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioManager.Instance?.PlayLevelMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void QuitLevel()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioManager.Instance?.PlayMenuMusic();
        SceneManager.LoadScene("menu");
    }
}