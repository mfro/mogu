using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System;

public class NavMenu : MonoBehaviour
{
    public event Action Options;
    public event Action Credits;

    [SerializeField] GameObject[] buttons;

    private GameObject selected;

    async void OnEnable()
    {
        await Task.Yield();
        EventSystem.current.SetSelectedGameObject(selected ?? buttons[0]);
    }

    public void DoPlay()
    {
        SceneController.instance.SwitchScene(1);
        selected = buttons[0];
    }

    public void DoOptions()
    {
        Options?.Invoke();
        selected = buttons[1];
    }

    public void DoCredits()
    {
        Credits?.Invoke();
        selected = buttons[2];
    }

    public void DoQuit()
    {
        Application.Quit();
        selected = buttons[3];
    }
}
