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


    private float maxMusic, maxMaster, maxSFX;

    public AudioMixer audioMixer;

    private void Start()
    {
        audioMixer.GetFloat("MusicVolume", out maxMusic);
        audioMixer.GetFloat("SFXVolume", out maxSFX);
        audioMixer.GetFloat("MasterVolume", out maxMaster);
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

        float volumeOfMixer = Mathf.Lerp(-80, maxMusic, volume / 100f);
        audioMixer.SetFloat("MusicVolume", volumeOfMixer);
        musicVolumeText.text = volume.ToString();
    }

    public void SetSFXVolume(float volume)
    {
        float volumeOfMixer = Mathf.Lerp(-80, maxSFX, volume / 100f);
        audioMixer.SetFloat("SFXVolume", volumeOfMixer);
        sfxVolumeText.text = volume.ToString();
    }

    public void SetMasterVolume(float volume)
    {
        float volumeOfMixer = Mathf.Lerp(-80, maxMaster, volume / 100f);
        audioMixer.SetFloat("MasterVolume", volumeOfMixer);
        masterVolumeText.text = volume.ToString();
    }

}
