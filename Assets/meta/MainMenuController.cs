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

    public AudioMixer audioMixer;

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

        float volumeOfMixer = Mathf.Lerp(-80, 20, volume / 100f);
        audioMixer.SetFloat("MusicVolume", volumeOfMixer);
    }

    public void SetSFXVolume(float volume)
    {
        float volumeOfMixer = Mathf.Lerp(-80, 20, volume / 100f);
        audioMixer.SetFloat("SFXVolume", volumeOfMixer);
    }

    public void SetMasterVolume(float volume)
    {
        float volumeOfMixer = Mathf.Lerp(-80, 20, volume / 100f);
        audioMixer.SetFloat("MasterVolume", volumeOfMixer);
    }

}
