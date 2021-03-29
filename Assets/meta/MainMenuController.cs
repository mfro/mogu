using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] GameObject MainScreen;
    [SerializeField] GameObject OptionsScreen;


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
    }

    public void SetSFXVolume(float volume)
    {
        float volumeOfMixer = Mathf.Lerp(-80, maxSFX, volume / 100f);
        audioMixer.SetFloat("SFXVolume", volumeOfMixer);
    }

    public void SetMasterVolume(float volume)
    {
        float volumeOfMixer = Mathf.Lerp(-80, maxMaster, volume / 100f);
        audioMixer.SetFloat("MasterVolume", volumeOfMixer);
    }

}
