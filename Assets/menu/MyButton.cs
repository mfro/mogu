using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System.Linq;

public class MyButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite hoverImage, pressImage;
    public Sprite pressCursor;

    private Image image, cursor;
    private Text text;

    private Button button;
    private Sprite baseImage;
    private Sprite baseCursor;
    private bool hover;

    void Awake()
    {
        Util.GetComponent(this, out button);

        var images = gameObject.GetChildren<Image>().ToList();
        image = images[0];
        cursor = images[1];

        text = GetComponentInChildren<Text>();

        baseImage = image.sprite;
        baseCursor = cursor.sprite;
    }

    void OnEnable()
    {
        image.sprite = baseImage;
        cursor.sprite = baseCursor;
        cursor.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.sprite = pressImage;
        cursor.sprite = pressCursor;
        text.transform.position += new Vector3(0, -4 * transform.lossyScale.y, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (image.sprite == pressImage)
        {
            image.sprite = hover ? hoverImage : baseImage;
            cursor.sprite = baseCursor;
            text.transform.position -= new Vector3(0, -4 * transform.lossyScale.y, 0);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hover = true;
        if (image.sprite == baseImage)
            image.sprite = hoverImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hover = false;
        if (image.sprite == hoverImage)
            image.sprite = baseImage;
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
