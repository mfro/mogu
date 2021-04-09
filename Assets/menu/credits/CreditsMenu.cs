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
        SetSelected();
    }

    async void SetSelected()
    {
        EventSystem.current.SetSelectedGameObject(null);
        await Task.Yield();
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void DoClose()
    {
        Close?.Invoke();
    }
}
