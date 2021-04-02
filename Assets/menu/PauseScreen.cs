using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PauseScreen : MonoBehaviour
{
    [SerializeField] LevelController controller;
    [SerializeField] GameObject pauseMenu;

    private bool isPaused = false;

    private void Start()
    {
        controller.PausePressed += OnPauseToggled;
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    public void OnQuit()
    {
        Physics.ClearPauseSet();
        SceneController.sceneController.SwitchScene(0);
    }

    public void OnOptions()
    {

    }

    private bool pausePressed = false;

    public void OnPauseToggled()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        return;
    }

    public void OnDestroy()
    {
        controller.PausePressed -= OnPauseToggled;
    }

}
