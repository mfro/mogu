using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainScreen;
    [SerializeField] OptionsScreen optionsScreen;
    [SerializeField] GameObject creditsScreen;

    [SerializeField] Audio mainMenuMusic;

    [SerializeField] Audio pressButtonSound;

    [SerializeField] GameObject[] buttons;
    [SerializeField] GameObject creditsReturnButton;
    [SerializeField] GameObject title;

    void Start()
    {
        AudioManager.audioManger?.PlayMusic(mainMenuMusic);
        optionsScreen.Close += DoOptionsReturn;
    }

    private void OnEnable()
    {
        title.SetActive(true);
        SetSelected(buttons[0]);
    }

    public async void SetSelected(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        await Task.Yield();
        EventSystem.current.SetSelectedGameObject(button);
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
        title.SetActive(false);
        AudioManager.audioManger?.PlaySFX(pressButtonSound);
        creditsScreen.SetActive(true);
        mainScreen.SetActive(false);
        SetSelected(creditsReturnButton);
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
        SetSelected(buttons[0]);
    }

    public void DoCreditsReturn()
    {
        title.SetActive(true);
        AudioManager.audioManger?.PlaySFX(pressButtonSound);
        mainScreen.SetActive(true);
        creditsScreen.SetActive(false);
        SetSelected(buttons[0]);
    }
}
