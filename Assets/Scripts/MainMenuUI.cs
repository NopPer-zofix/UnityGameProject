using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject leaderboardPanel;

    [Header("Main")]
    public Button playBtn;
    public Button settingsBtn;
    public Button leaderboardBtn;
    public Button quitBtn;
    public TextMeshProUGUI bestScoreTxt;

    [Header("Settings")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button settingsBackBtn;

    [Header("Leaderboard")]
    public TextMeshProUGUI leaderboardTxt;
    public Button leaderboardBackBtn;

    const string BEST_SCORE_KEY = "BestScore";
    const string MUSIC_KEY = "MusicVol";
    const string SFX_KEY = "SFXVol";

    void Start()
    {
        playBtn.onClick.AddListener(OnPlay);
        settingsBtn.onClick.AddListener(() => ShowPanel(settingsPanel));
        leaderboardBtn.onClick.AddListener(OnLeaderboard);
        quitBtn.onClick.AddListener(Application.Quit);
        settingsBackBtn.onClick.AddListener(() => ShowPanel(mainPanel));
        leaderboardBackBtn.onClick.AddListener(() => ShowPanel(mainPanel));

        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        sfxSlider.value   = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        musicSlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat(MUSIC_KEY, v));
        sfxSlider.onValueChanged.AddListener(v  => PlayerPrefs.SetFloat(SFX_KEY, v));

        int best = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        bestScoreTxt.text = $"Best result: {best:N0} points";

        ShowPanel(mainPanel);
    }

    void OnPlay() => SceneManager.LoadScene("GameScene");

    void OnLeaderboard()
    {
        // Показываем топ-5 (храним в PlayerPrefs как JSON или отдельными ключами)
        var sb = new System.Text.StringBuilder();
        for (int i = 1; i <= 5; i++)
        {
            int s = PlayerPrefs.GetInt($"Score_{i}", 0);
            sb.AppendLine(s > 0 ? $"{i}. {s:N0}" : $"{i}. ---");
        }
        leaderboardTxt.text = sb.ToString();
        ShowPanel(leaderboardPanel);
    }

    void ShowPanel(GameObject target)
    {
        mainPanel.SetActive(target == mainPanel);
        settingsPanel.SetActive(target == settingsPanel);
        leaderboardPanel.SetActive(target == leaderboardPanel);
    }
}
