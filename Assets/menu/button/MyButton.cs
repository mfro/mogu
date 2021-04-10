using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System.Linq;

public class MyButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler
{
    public Sprite hoverImage, pressImage;
    public Sprite pressCursor;

    public Audio pressSound;

    private Image image, cursor;
    private Text text;

    private Button button;
    private Animator animator;
    private Sprite baseImage;
    private Sprite baseCursor;
    private bool hover;
    private bool isEnabled;
    public bool alreadyPressed = false;

    void Awake()
    {
        Util.GetComponent(this, out button);
        Util.GetComponent(this, out animator);

        var images = gameObject.GetChildren<Image>().ToList();
        image = images.Find(a => a.name == "base");
        cursor = images.Find(a => a.name == "cursor");

        text = GetComponentInChildren<Text>();

        baseImage = image.sprite;
        baseCursor = cursor.sprite;

        button.onClick.AddListener(OnClick);
        alreadyPressed = false;
    }

    private void OnClick()
    {
        AudioManager.instance?.PlaySFX(pressSound);
    }

    void OnEnable()
    {
        isEnabled = true;
        image.sprite = baseImage;
        cursor.sprite = baseCursor;
        if (gameObject == EventSystem.current.currentSelectedGameObject)
        {
            cursor.gameObject.SetActive(true);
            animator.Rebind();
            animator.Update(0);
        }
        else
        {
            cursor.gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        isEnabled = false;
        if (image.sprite == pressImage)
        {
            image.sprite = baseImage;
            cursor.sprite = baseCursor;
            animator.SetTrigger("isUnPressed");
            text.transform.position -= new Vector3(0, -4 * transform.lossyScale.y, 0);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.sprite = pressImage;
        cursor.sprite = pressCursor;
        animator.SetTrigger("isPressed");
        text.transform.position += new Vector3(0, -4 * transform.lossyScale.y, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (image.sprite == pressImage)
        {
            image.sprite = hover ? hoverImage : baseImage;
            cursor.sprite = baseCursor;
            animator.SetTrigger("isUnPressed");
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
        animator.Rebind();
        animator.Update(0);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        cursor.gameObject.SetActive(false);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (!isEnabled) return;

        isEnabled = false;

        image.sprite = pressImage;
        cursor.sprite = pressCursor;
        animator.SetTrigger("isPressed");
        text.transform.position += new Vector3(0, -4 * transform.lossyScale.y, 0);
    }
}
