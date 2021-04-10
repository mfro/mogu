using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System;

public class NavMenu : MonoBehaviour
{
    public event Action Play;
    public event Action Options;
    public event Action Credits;

    [SerializeField] GameObject[] buttons;

    private GameObject selected;

    void OnEnable()
    {
        canPressPlay = true;
        canPressOptions = true;
        canPressCredits = true;
        canPressQuit = true;
        EventSystem.current.SetSelectedGameObject(selected ?? buttons[0]);
    }

    private bool canPressPlay = true;
    public void DoPlay()
    {
        if (!canPressPlay) return;
        canPressPlay = false;
        Play?.Invoke();
        // SceneController.instance.SwitchScene(1);
        selected = buttons[0];
    }

    private bool canPressOptions = true;
    public void DoOptions()
    {
        if (!canPressOptions) return;
        canPressOptions = false;
        Options?.Invoke();
        selected = buttons[1];
    }

    private bool canPressCredits = true;
    public void DoCredits()
    {
        if (!canPressCredits) return;
        canPressCredits = false;
        Credits?.Invoke();
        selected = buttons[2];
    }

    private bool canPressQuit = true;
    public void DoQuit()
    {
        if (!canPressQuit) return;
        canPressQuit = false;
        Application.Quit();
        selected = buttons[3];
    }
}
