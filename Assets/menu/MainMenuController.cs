using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainScreen;
    [SerializeField] OptionsScreen optionsScreen;

    [SerializeField] Audio mainMenuMusic;

    [SerializeField] Audio pressButtonSound;

    void Start()
    {
        AudioManager.audioManger?.PlayMusic(mainMenuMusic);
        optionsScreen.Close += DoOptionsReturn;
    }

    public void DoPlay()
    {
        AudioManager.audioManger?.PlaySFX(pressButtonSound);
        SceneController.sceneController.SwitchScene(1);
    }

    public void DoOptions()
    {
        AudioManager.audioManger?.PlaySFX(pressButtonSound);
        mainScreen.SetActive(false);
        optionsScreen.gameObject.SetActive(true);
    }

    public void DoCredits()
    {
        AudioManager.audioManger?.PlaySFX(pressButtonSound);
        print("credits");
    }

    public void DoQuit()
    {
        Application.Quit();
    }

    public void DoOptionsReturn()
    {
        AudioManager.audioManger?.PlaySFX(pressButtonSound);
        mainScreen.SetActive(true);
        optionsScreen.gameObject.SetActive(false);
    }
}
