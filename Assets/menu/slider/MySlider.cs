using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System.Linq;

public class MySlider : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Image cursor;

    void Awake()
    {
        var images = gameObject.GetChildren<Image>().ToList();
    }

    void OnEnable()
    {
        cursor.gameObject.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        cursor.gameObject.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        cursor.gameObject.SetActive(false);
    }
}
