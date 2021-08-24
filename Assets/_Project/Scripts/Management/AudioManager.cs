using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    private float m_musicVolume;

    public float MusicVolume
    {
        get
        {
            return m_musicVolume;
        }

        set
        {
            value = Mathf.Clamp(value, 0, 1);
            m_backgroundMusic.volume = m_musicVolume;
            m_musicVolume = value;
        }
    }

    public float MusicVolumeSave
    {
        get
        {
            return m_musicVolume;
        }

        set
        {
            value = Mathf.Clamp(value, 0, 1);
            m_backgroundMusic.volume = m_musicVolume;
            PlayerPrefs.SetFloat(AppPlayerPrefKeys.MUSIC_VOLUME, value);
            m_musicVolume = value;
        }
    }


    private float m_sfxVolume;

    public float SfxVolume
    {
        get
        {
            return m_sfxVolume;
        }

        set
        {
            value = Mathf.Clamp(value, 0, 1);
            m_sfxMusic.volume = m_sfxVolume;
            m_sfxVolume = value;
        }
    }

    public float SfxVolumeSave
    {
        get
        {
            return m_sfxVolume;
        }

        set
        {
            value = Mathf.Clamp(value, 0, 1);
            m_sfxMusic.volume = m_sfxVolume;
            PlayerPrefs.SetFloat(AppPlayerPrefKeys.SFX_VOLUME, value);
            m_sfxVolume = value;
        }
    }


    public override void Awake()
    {
        base.Awake();

        m_soundFXDictionary = new Dictionary<string, AudioClip>();
        m_soundMusicDictionary = new Dictionary<string, AudioClip>();

        m_backgroundMusic = CreateAudioSource("Music", true);
        m_sfxMusic = CreateAudioSource("Sfx", false);

        MusicVolume = PlayerPrefs.GetFloat(AppPlayerPrefKeys.MUSIC_VOLUME, 0.5f);
        SfxVolume = PlayerPrefs.GetFloat(AppPlayerPrefKeys.SFX_VOLUME, 0.5f);

        AudioClip[] audioSfxVector = Resources.LoadAll<AudioClip>(AppPaths.PATH_RESOURCE_SFX);

        for (int i = 0; i < audioSfxVector.Length; i++)
        {
            m_soundFXDictionary.Add(audioSfxVector[i].name, audioSfxVector[i]);
        }

        audioSfxVector = Resources.LoadAll<AudioClip>(AppPaths.PATH_RESOURCE_MUSIC);

        for (int i = 0; i < audioSfxVector.Length; i++)
        {
            m_soundMusicDictionary.Add(audioSfxVector[i].name, audioSfxVector[i]);
        }
    }


    public void PlayBackgroundMusic(string audioName)
    {
        if (m_soundMusicDictionary.ContainsKey(audioName))
        {
            m_backgroundMusic.clip = m_soundMusicDictionary[audioName];
            m_backgroundMusic.volume = m_musicVolume;
            m_backgroundMusic.Play();
        }
    }

    public void PlaySound(string audioName)
    {
        if (m_soundFXDictionary.ContainsKey(audioName))
        {
            m_sfxMusic.clip = m_soundFXDictionary[audioName];
            m_sfxMusic.volume = m_sfxVolume;
            m_sfxMusic.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (m_backgroundMusic != null)
            m_backgroundMusic.Stop();
    }

    public void PauseBackgroundMusic()
    {
        if (m_backgroundMusic != null)
            m_backgroundMusic.Pause();
    }

    private AudioSource CreateAudioSource(string name, bool isLoop)
    {
        GameObject temporaryAudioHost = new GameObject(name);
        AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
        audioSource.playOnAwake = false;
        audioSource.loop = isLoop;
        audioSource.spatialBlend = 0.0f;
        temporaryAudioHost.transform.SetParent(this.transform);
        return audioSource;
    }

    private Dictionary<string, AudioClip> m_soundFXDictionary = null;
    private Dictionary<string, AudioClip> m_soundMusicDictionary = null;
    private AudioSource m_backgroundMusic = null;
    private AudioSource m_sfxMusic = null;

}