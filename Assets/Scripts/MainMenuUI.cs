using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject quitConfirmPanel;

    [Header("Main Buttons")]
    public Button playBtn;
    public Button settingsBtn;
    public Button quitBtn;

    [Header("Settings")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button settingsBackBtn;

    [Header("Quit Confirm")]
    public Button quitYesBtn;
    public Button quitNoBtn;

    void Start()
    {
        playBtn.onClick.AddListener(OnPlay);
        settingsBtn.onClick.AddListener(() => ShowPanel(settingsPanel));
        quitBtn.onClick.AddListener(() => ShowPanel(quitConfirmPanel));
        settingsBackBtn.onClick.AddListener(() => ShowPanel(mainPanel));
        quitYesBtn.onClick.AddListener(OnQuitConfirmed);
        quitNoBtn.onClick.AddListener(() => ShowPanel(mainPanel));

        // Load saved volume and hook sliders to AudioManager
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 1f);
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;
        musicSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetMusicVolume(v));
        sfxSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetSFXVolume(v));

        // Start menu music
        AudioManager.Instance?.PlayMenuMusic();

        ShowPanel(mainPanel);
    }

    void OnPlay()
    {
        AudioManager.Instance?.PlayLevelMusic();
        SceneManager.LoadScene("Level_1");
    }

    void OnQuitConfirmed()
    {
        // Quit the application entirely
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void ShowPanel(GameObject target)
    {
        mainPanel.SetActive(target == mainPanel);
        settingsPanel.SetActive(target == settingsPanel);
        quitConfirmPanel.SetActive(target == quitConfirmPanel);
    }
}