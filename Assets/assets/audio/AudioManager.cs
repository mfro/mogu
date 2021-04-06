using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Audio
{
    public AudioClip audioClip;
    public float volume;
    public bool looping;
    public AudioMixerGroup mixerGroup;

    public Audio(AudioClip audioClip, float volume = 1.0f, bool looping = false, AudioMixerGroup mixerGroup = null)
    {
        this.audioClip = audioClip;
        this.volume = volume;
        this.looping = looping;
        this.mixerGroup = mixerGroup;
    }
}

public class AudioGroup
{
    public string Name { get; }

    public float Min { get; }
    public float Max { get; }
    public float DefaultValue { get; }

    public float GetMixerVolume(float value) => value == 0 ? -80 : Mathf.Lerp(Min, Max, value / 100);

    public AudioGroup(string name, float min, float max, float defaultValue)
    {
        Name = name;
        Min = min;
        Max = max;
        DefaultValue = defaultValue;
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioGroup
        Master = new AudioGroup("MasterVolume", -35, 0, 100),
        Effects = new AudioGroup("SFXVolume", -35, 0, 100),
        Music = new AudioGroup("MusicVolume", -35, 0, 50);

    public static AudioManager instance;

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [SerializeField] AudioMixer audioMixer;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            transform.SetParent(null, false);
            DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
        SetValue(Master, GetValue(Master));
        SetValue(Effects, GetValue(Effects));
        SetValue(Music, GetValue(Music));
    }

    public void PlayMusic(Audio audio)
    {
        if (audio.audioClip == musicSource.clip && musicSource.isPlaying) return;

        musicSource.clip = audio.audioClip;
        musicSource.loop = audio.looping;
        musicSource.volume = audio.volume;
        musicSource.outputAudioMixerGroup = audio.mixerGroup;
        musicSource.Play();
    }

    public void PlaySFX(Audio audio)
    {
        if (audio.audioClip == SFXSource.clip && SFXSource.isPlaying) return;

        SFXSource.clip = audio.audioClip;
        SFXSource.loop = audio.looping;
        SFXSource.volume = audio.volume;
        SFXSource.outputAudioMixerGroup = audio.mixerGroup;
        SFXSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetValue(AudioGroup group, float value)
    {
        var volume = group.GetMixerVolume(value);

        audioMixer.SetFloat(group.Name, volume);
        PlayerPrefs.SetFloat(group.Name, value);
    }

    public float GetValue(AudioGroup group)
    {
        return PlayerPrefs.HasKey(group.Name)
            ? PlayerPrefs.GetFloat(group.Name)
            : group.DefaultValue;
    }
}
