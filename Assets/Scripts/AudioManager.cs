using UnityEngine;

// Singleton — survives scene loads, controls all music/sfx volume globally
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;

    [Header("Clips")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;

    [Range(0f, 1f)] public float normalVolume  = 1f;
    [Range(0f, 1f)] public float dimmedVolume  = 0.25f; // volume when pause menu is open

    float targetVolume;
    float lerpSpeed = 4f;

    void Awake()
    {
        // Singleton pattern — only one AudioManager exists at all times
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        normalVolume = PlayerPrefs.GetFloat("MusicVol", 1f);
        targetVolume = normalVolume;
        musicSource.volume = normalVolume;
    }

    void Update()
    {
        // Smoothly lerp toward target volume
        if (!Mathf.Approximately(musicSource.volume, targetVolume))
            musicSource.volume = Mathf.Lerp(musicSource.volume, targetVolume, Time.unscaledDeltaTime * lerpSpeed);
    }

    public void PlayMenuMusic()
    {
        if (musicSource.clip == menuMusic && musicSource.isPlaying) return;
        musicSource.clip = menuMusic;
        musicSource.loop = true;
        musicSource.Play();
        Undim();
    }

    public void PlayLevelMusic()
    {
        if (musicSource.clip == levelMusic && musicSource.isPlaying) return;
        musicSource.clip = levelMusic;
        musicSource.loop = true;
        musicSource.Play();
        Undim();
    }

    // Call when pause menu opens
    public void Dim()   => targetVolume = dimmedVolume;

    // Call when pause menu closes
    public void Undim() => targetVolume = normalVolume;

    // Called by music slider in UI
    public void SetMusicVolume(float v)
    {
        normalVolume = v;
        PlayerPrefs.SetFloat("MusicVol", v);
        targetVolume = v;
    }

    // Called by sfx slider in UI
    public void SetSFXVolume(float v)
    {
        PlayerPrefs.SetFloat("SFXVol", v);
        // Hook up your SFX AudioSource here if needed
    }
}
