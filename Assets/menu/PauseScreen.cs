using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] GameObject mainScreen;
    [SerializeField] OptionsScreen optionsScreen;

    [SerializeField] Audio pressButtonSound;

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
        optionsScreen.gameObject.SetActive(false);

        if (isPaused)
        {
            AudioManager.audioManger.PlaySFX(pressButtonSound);
        }
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
    }

    private bool _onPause = false;
    public void OnPause(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onPause && FlipPanel.isFlipping == null)
            TogglePause();

        _onPause = c.ReadValueAsButton();
    }
}
