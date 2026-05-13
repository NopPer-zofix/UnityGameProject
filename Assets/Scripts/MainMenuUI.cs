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
    public TextMeshProUGUI bestScoreTxt;

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

        quitYesBtn.onClick.AddListener(Application.Quit);
        quitNoBtn.onClick.AddListener(() => ShowPanel(mainPanel));

        // Load saved volume settings
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 1f);
        musicSlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat("MusicVol", v));
        sfxSlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat("SFXVol", v));

        // Display best score
        int best = PlayerPrefs.GetInt("BestScore", 0);
        bestScoreTxt.text = best > 0 ? $"Best Score: {best:N0}" : "Best Score: ---";

        ShowPanel(mainPanel);
    }

    void OnPlay() => SceneManager.LoadScene("Level_1");

    void ShowPanel(GameObject target)
    {
        mainPanel.SetActive(target == mainPanel);
        settingsPanel.SetActive(target == settingsPanel);
        quitConfirmPanel.SetActive(target == quitConfirmPanel);
    }
}