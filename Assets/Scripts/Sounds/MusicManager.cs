using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField]
    private MusicLibrary musicLibrary;
    [SerializeField]
    private AudioSource musicSource;

    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float fadeVolumeFactor = 1f;
    private Coroutine crossfadeCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // Load initial volume settings from PlayerPrefs
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        UpdateVolume();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        UpdateVolume();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        if (musicSource != null)
        {
            musicSource.volume = fadeVolumeFactor * masterVolume * musicVolume;
        }
    }

    public void PlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        if (crossfadeCoroutine != null)
        {
            StopCoroutine(crossfadeCoroutine);
        }
        crossfadeCoroutine = StartCoroutine(AnimateMusicCrossFade(musicLibrary.GetClipFromName(trackName), fadeDuration));
    }

    IEnumerator AnimateMusicCrossFade(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        float percent = 0;
        float startFadeFactor = fadeVolumeFactor;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            fadeVolumeFactor = Mathf.Lerp(startFadeFactor, 0, percent);
            UpdateVolume();
            yield return null;
        }
        musicSource.clip = nextTrack;
        musicSource.Play();

        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            fadeVolumeFactor = Mathf.Lerp(0, 1f, percent);
            UpdateVolume();
            yield return null;
        }
        crossfadeCoroutine = null;
    }
}
