using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] GameObject mainScreen;
    [SerializeField] OptionsScreen optionsScreen;

    [SerializeField] Audio pressButtonSound;

    [SerializeField] public CanvasGroup LevelTitle;


    [SerializeField] GameObject[] buttons;

    private bool isPaused = false;

    void Start()
    {
        optionsScreen.Close += DoOptionsReturn;
    }

    public void TogglePause()
    {
        if (isPaused == Physics.IsEnabled) return;

        isPaused = !isPaused;

        Physics.IsEnabled = !isPaused;
        gameObject.SetActive(isPaused);
        mainScreen.SetActive(isPaused);
        LevelTitle.alpha = isPaused ? 1 : 0;
        LevelTitle.gameObject.SetActive(isPaused);
        optionsScreen.gameObject.SetActive(false);

        if (isPaused)
        {
            AudioManager.audioManger.PlaySFX(pressButtonSound);
            SetSelected();
        } 
    }

    async void SetSelected()
    {
        EventSystem.current.SetSelectedGameObject(null);

        await Task.Yield();

        EventSystem.current.SetSelectedGameObject(buttons[0]);
    }

    public void DoQuit()
    {
        SceneController.sceneController.SwitchScene(0);
        AudioManager.audioManger.PlaySFX(pressButtonSound);
    }

    public void DoOptions()
    {
        mainScreen.SetActive(false);
        optionsScreen.gameObject.SetActive(true);
        AudioManager.audioManger.PlaySFX(pressButtonSound);
    }

    public void DoOptionsReturn()
    {
        mainScreen.SetActive(true);
        optionsScreen.gameObject.SetActive(false);
        AudioManager.audioManger.PlaySFX(pressButtonSound);

        SetSelected();
    }

    private bool _onPause = false;
    public void OnPause(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onPause && FlipPanel.isFlipping == null)
            TogglePause();

        _onPause = c.ReadValueAsButton();
    }
}
