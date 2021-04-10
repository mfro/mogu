using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;

public class NavMenu : MonoBehaviour
{
    public event Action Play;
    public event Action Options;
    public event Action Credits;

    [SerializeField] GameObject[] buttons;

    private GameObject selected;

    void OnEnable()
    {
        SetAllButtonsEnabled(true);
        EventSystem.current.SetSelectedGameObject(selected ?? buttons[0]);
    }

    public void DoPlay()
    {
        Play?.Invoke();
        // SceneController.instance.SwitchScene(1);
        selected = buttons[0];
        SetAllButtonsEnabled(false);
    }

    public void DoOptions()
    {
        Options?.Invoke();
        selected = buttons[1];
        SetAllButtonsEnabled(false);
    }

    public void DoCredits()
    {
        Credits?.Invoke();
        selected = buttons[2];
        SetAllButtonsEnabled(false);
    }

    public void DoQuit()
    {
        Application.Quit();
        selected = buttons[3];
        SetAllButtonsEnabled(false);
    }

    public void SetAllButtonsEnabled(bool enabled)
    {
        foreach (var button in buttons)
        {
            button.GetComponent<Button>().interactable = enabled;
        }
    }
}
