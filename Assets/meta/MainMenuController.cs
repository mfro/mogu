using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] GameObject MainScreen;
    [SerializeField] GameObject OptionsScreen;
    [SerializeField] TextMeshProUGUI musicVolumeText, masterVolumeText, sfxVolumeText;
    [SerializeField] Audio mainMenuMusic;

    private float maxMusic, maxMaster, maxSFX;

    [SerializeField] AudioMixer audioMixer;

    private void Start()
    {
        audioMixer.GetFloat("MusicVolume", out maxMusic);
        audioMixer.GetFloat("SFXVolume", out maxSFX);
        audioMixer.GetFloat("MasterVolume", out maxMaster);
        MusicManager.musicManager.PlayMusic(mainMenuMusic);
    }

    public void OnPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void OnOptions()
    {
        MainScreen.SetActive(false);
        OptionsScreen.SetActive(true);
    }

    public void OnCredits()
    {
        print("credits");
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    public void OnOptionsReturn()
    {
        MainScreen.SetActive(true);
        OptionsScreen.SetActive(false);
    }
    

    public void SetMusicVolume(float volume)
    {

        float volumeOfMixer = Mathf.Lerp(-35f, maxMusic, volume / 100f);
        volumeOfMixer = volume == 0 ? -80f : volumeOfMixer;

        audioMixer.SetFloat("MusicVolume", volumeOfMixer);
        musicVolumeText.text = volume.ToString();
    }

    public void SetSFXVolume(float volume)
    {
        float volumeOfMixer = Mathf.Lerp(-35f, maxSFX, volume / 100f);
        volumeOfMixer = volume == 0 ? -80f : volumeOfMixer;

        audioMixer.SetFloat("SFXVolume", volumeOfMixer);
        sfxVolumeText.text = volume.ToString();
    }

    public void SetMasterVolume(float volume)
    {
        float volumeOfMixer = Mathf.Lerp(-35f, maxMaster, volume / 100f);
        volumeOfMixer = volume == 0 ? -80f : volumeOfMixer;

        audioMixer.SetFloat("MasterVolume", volumeOfMixer);
        masterVolumeText.text = volume.ToString();
    }

}
