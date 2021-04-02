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

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManger;

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    // Start is called before the first frame update
    void Awake()
    {
        if (audioManger == null)
        {
            audioManger = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    public void PlayMusic (Audio audio)
    {
        if (audio.audioClip == musicSource.clip && musicSource.isPlaying) return;

        musicSource.clip = audio.audioClip;
        musicSource.loop = audio.looping;
        musicSource.volume = audio.volume;
        musicSource.outputAudioMixerGroup = audio.mixerGroup;
        musicSource.Play();
    }

    public void PlaySFX (Audio audio)
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
}
