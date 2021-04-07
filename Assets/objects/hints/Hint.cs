using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum InputMode
{
    Keyboard,
    Controller,
}

public class Hint : MonoBehaviour
{
    public static InputMode mode = InputMode.Keyboard;

    public Sprite keyboard;
    public Sprite controller;

    private new SpriteRenderer renderer;

    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void ReRender()
    {
        Render();
    }

    private void Render()
    {
        switch (mode)
        {
            case InputMode.Keyboard: renderer.sprite = keyboard; break;
            case InputMode.Controller: renderer.sprite = controller; break;
        }
    }
}
