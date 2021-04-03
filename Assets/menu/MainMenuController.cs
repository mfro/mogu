using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainScreen;
    [SerializeField] OptionsScreen optionsScreen;

    [SerializeField] Audio mainMenuMusic;

    [SerializeField] Audio pressButtonSound;

    [SerializeField] GameObject[] buttons;

    void Start()
    {
        AudioManager.audioManger?.PlayMusic(mainMenuMusic);
        optionsScreen.Close += DoOptionsReturn;
    }

    private void OnEnable()
    {
        SetSelected();
    }

    public async void SetSelected()
    {
        EventSystem.current.SetSelectedGameObject(null);
        await Task.Yield();
        EventSystem.current.SetSelectedGameObject(buttons[0]);
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
        SetSelected();
    }
}
