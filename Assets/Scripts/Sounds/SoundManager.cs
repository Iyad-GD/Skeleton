using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField]
    private SoundLibrary sfxLibrary;
    [SerializeField]
    private AudioSource sfx2DSources;

    private float masterVolume = 1f;
    private float sfxVolume = 1f;

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
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        UpdateVolume();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        UpdateVolume();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        if (sfx2DSources != null)
        {
            sfx2DSources.volume = masterVolume * sfxVolume;
        }
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, masterVolume * sfxVolume);
        }
    }
    public void PlaySound3D(string soundName, Vector3 pos)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(soundName), pos);
    }
    public void PlaySound2D(string soundName)
    {
        if (sfxLibrary != null)
        {
            AudioClip clip = sfxLibrary.GetClipFromName(soundName);
            if (clip != null && sfx2DSources != null)
            {
                sfx2DSources.PlayOneShot(clip);
            }
        }
    }
}
