using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class CreditsMenu : MonoBehaviour
{
    public event Action Close;

    [SerializeField] GameObject button;

    void OnEnable()
    {
        canPressClose = true;
        EventSystem.current.SetSelectedGameObject(button);
    }

    private bool canPressClose = true;
    public void DoClose()
    {
        if (!canPressClose) return;
        canPressClose = false;
        Close?.Invoke();
    }
}
