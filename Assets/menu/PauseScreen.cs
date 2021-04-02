using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using TMPro;


public class PauseScreen : MonoBehaviour
{
    [SerializeField] LevelController controller;
    [SerializeField] GameObject pauseScreen;

    [SerializeField] GameObject mainScreen, optionsScreen;

    private float maxMusic, maxMaster, maxSFX;
    [SerializeField] TextMeshProUGUI musicVolumeText, masterVolumeText, sfxVolumeText;

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Audio pressButtonSound;

    private bool isPaused = false;

    private void Start()
    {
        controller.PausePressed += OnPauseToggled;
        pauseScreen.SetActive(false);
        mainScreen.SetActive(false);
        optionsScreen.SetActive(false);
        isPaused = false;
    }

    public void OnQuit()
    {
        SceneController.sceneController.SwitchScene(0);
        AudioManager.audioManger.PlaySFX(pressButtonSound);
    }

    public void OnOptions()
    {
        optionsScreen.SetActive(true);
        mainScreen.SetActive(false);
        AudioManager.audioManger.PlaySFX(pressButtonSound);
    }

    public void OnPauseToggled()
    {
        isPaused = !isPaused;
        pauseScreen.SetActive(isPaused);
        mainScreen.SetActive(isPaused);
        optionsScreen.SetActive(false);
        AudioManager.audioManger.PlaySFX(pressButtonSound);
        return;
    }

    public void OnOptionsReturn()
    {
        AudioManager.audioManger?.PlaySFX(pressButtonSound);
        mainScreen.SetActive(true);
        optionsScreen.SetActive(false);
        AudioManager.audioManger.PlaySFX(pressButtonSound);
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

    public void EndDrag()
    {
        AudioManager.audioManger.PlaySFX(pressButtonSound);
    }

    public void OnDestroy()
    {
        controller.PausePressed -= OnPauseToggled;
    }

}
