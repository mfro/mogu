using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] GameObject nav;
    [SerializeField] OptionsMenu options;
    [SerializeField] GameObject optionsScreenBackdrop;

    [SerializeField] Audio pressButtonSound;

    [SerializeField] public CanvasGroup LevelTitle;


    [SerializeField] GameObject[] buttons;

    private bool isPaused = false;

    void Start()
    {
        options.Close += DoOptionsReturn;
    }

    public void TogglePause()
    {
        if (isPaused == Physics.IsEnabled) return;

        isPaused = !isPaused;

        Physics.IsEnabled = !isPaused;
        gameObject.SetActive(isPaused);
        nav.SetActive(isPaused);
        LevelTitle.alpha = 1;
        LevelTitle.gameObject.SetActive(isPaused);
        options.gameObject.SetActive(false);
        optionsScreenBackdrop.gameObject.SetActive(false);

        if (isPaused)
        {
            AudioManager.instance.PlaySFX(pressButtonSound);
            EventSystem.current.SetSelectedGameObject(buttons[0]);
            canPressQuit = true;
            canPressOptions = true;
            canPressOptionsReturn = true;
        }
    }


    private bool canPressQuit = true;
    public void DoQuit()
    {
        if (!canPressQuit) return;
        canPressQuit = false;
        SceneController.instance.SwitchScene(0);
        AudioManager.instance.PlaySFX(pressButtonSound);
    }

    private bool canPressOptions = true;
    public void DoOptions()
    {
        if (!canPressOptions) return;
        canPressOptions = true;
        nav.SetActive(false);
        LevelTitle.gameObject.SetActive(false);
        options.gameObject.SetActive(true);
        optionsScreenBackdrop.gameObject.SetActive(true);
        AudioManager.instance.PlaySFX(pressButtonSound);
    }

    private bool canPressOptionsReturn = true;
    public void DoOptionsReturn()
    {
        if (!canPressOptionsReturn) return;
        canPressOptionsReturn = false;

        nav.SetActive(true);
        LevelTitle.gameObject.SetActive(true);
        options.gameObject.SetActive(false);
        optionsScreenBackdrop.gameObject.SetActive(false);
        AudioManager.instance.PlaySFX(pressButtonSound);

        EventSystem.current.SetSelectedGameObject(buttons[0]);
    }

    private bool _onPause = false;
    public void OnPause(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onPause && FlipPanel.isFlipping == null)
            TogglePause();

        _onPause = c.ReadValueAsButton();
    }
}
