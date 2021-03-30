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

    public static AudioManager audioManager;

    [SerializeField] AudioSource sourceMusic;
    //[SerializeField] AudioSource sourceSFX;

    // Start is called before the first frame update
    void Start()
    {
        if (audioManager == null)
        {
            audioManager = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    public void PlayMusic(Audio audio)
    {
        sourceMusic.clip = audio.audioClip;
        sourceMusic.loop = audio.looping;
        sourceMusic.volume = audio.volume;
        sourceMusic.outputAudioMixerGroup = audio.mixerGroup;
        sourceMusic.Play();
    }


    public void StopMusic()
    {
        sourceMusic.Stop();
    }

    //public void PlaySFX(Audio audio)
    //{
    //    sourceSFX.clip = audio.audioClip;
    //    sourceSFX.loop = audio.looping;
    //    sourceSFX.outputAudioMixerGroup = audio.mixerGroup;
    //    sourceSFX.volume = audio.volume;
    //    sourceSFX.Play();
    //}

    //public void StopSFX()
    //{
    //    sourceSFX.Stop();
    //}
}
