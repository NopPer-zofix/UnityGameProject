using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    public AudioMixer mainMixer; // drag MainMixer here

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;
    public AudioClip gameOverMusic;
    public AudioClip victoryMusic;

    [Header("SFX Clips")]
    public AudioClip PistolSFX;
    public AudioClip SmgSFX;
    public AudioClip ShotgunSFX;
    public AudioClip FootstepsSFX;
    public AudioClip EquipSFX;

    // Mixer exposed parameter names — must match exactly in AudioMixer
    const string MUSIC_PARAM = "MusicVol";
    const string SFX_PARAM = "SFXVol";

    bool initialized = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() => Init();

    void Init()
    {
        if (initialized) return;
        initialized = true;

        // Restore saved volumes into the mixer
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVol", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVol", 1f));
    }

    // ── Music ──────────────────────────────────────────────

    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayLevelMusic() => PlayMusic(levelMusic);
    public void PlayGameOver() => PlayMusic(gameOverMusic);
    public void PlayVictory() => PlayMusic(victoryMusic);

    void PlayMusic(AudioClip clip)
    {
        if (clip == null) { Debug.LogWarning("AudioManager: clip is null!"); return; }
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // ── Dim (pause menu) ───────────────────────────────────

    public void Dim()
    {
        // Lower music group by -12 dB when paused
        mainMixer?.SetFloat(MUSIC_PARAM, LinearToDecibel(PlayerPrefs.GetFloat("MusicVol", 1f) * 0.25f));
    }

    public void Undim()
    {
        mainMixer?.SetFloat(MUSIC_PARAM, LinearToDecibel(PlayerPrefs.GetFloat("MusicVol", 1f)));
    }

    // ── SFX ────────────────────────────────────────────────

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayPistol() => PlaySFX(PistolSFX);
    public void PlaySmg() => PlaySFX(SmgSFX);
    public void PlayShotgun() => PlaySFX(ShotgunSFX);
    public void PlayFootsteps() => PlaySFX(FootstepsSFX);
    public void PlayEquip() => PlaySFX(EquipSFX);

    // ── Volume (called by UI sliders) ──────────────────────

    // slider value: 0.0 – 1.0
    public void SetMusicVolume(float v)
    {
        PlayerPrefs.SetFloat("MusicVol", v);
        mainMixer?.SetFloat(MUSIC_PARAM, LinearToDecibel(v));
    }

    public void SetSFXVolume(float v)
    {
        PlayerPrefs.SetFloat("SFXVol", v);
        mainMixer?.SetFloat(SFX_PARAM, LinearToDecibel(v));
    }

    // Decibel conversion: slider 0→1 maps to -80→0 dB
    float LinearToDecibel(float linear)
    {
        return linear > 0.0001f ? Mathf.Log10(linear) * 20f : -80f;
    }
}