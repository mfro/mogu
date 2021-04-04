using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum InputMode
{
    Keyboard,
    Controller,
}

public enum HintInput
{
    Up,
    Down,
    Left,
    Right,
    Jump,
    Flip1,
    Flip2,
    Undo,
    Restart,
}

public enum HintContext
{
    None,
    CW,
    CCW,
    Vertical,
    Horizontal,
}

public class Hint : MonoBehaviour
{
    public static InputMode mode = InputMode.Keyboard;
    public float opacity;
    public HintInput input;
    public HintContext context;

    public Sprite background;

    public Sprite up, down, left, right;

    public Sprite space, z, f;
    public Sprite x, r;
    public Sprite l, a, b;

    public Sprite cw, ccw;
    public Sprite vertical, horizontal;

    private List<GameObject> children;

    public IEnumerable<Sprite> GetSprites()
    {
        if (input == HintInput.Up) yield return up;
        if (input == HintInput.Down) yield return down;
        if (input == HintInput.Left) yield return left;
        if (input == HintInput.Right) yield return right;

        if (input == HintInput.Jump)
            if (mode == InputMode.Keyboard) yield return space;
            else if (mode == InputMode.Controller) yield return a;

        if (input == HintInput.Flip1)
            if (mode == InputMode.Keyboard) yield return z;
            else if (mode == InputMode.Controller) yield return l;

        if (input == HintInput.Flip2)
            if (mode == InputMode.Keyboard) yield return x;
            else if (mode == InputMode.Controller) yield return r;

        if (input == HintInput.Undo)
            if (mode == InputMode.Keyboard) yield return f;
            else if (mode == InputMode.Controller) yield return x;

        if (input == HintInput.Restart)
            yield return r;

        switch (context)
        {
            case HintContext.CW: yield return cw; break;
            case HintContext.CCW: yield return ccw; break;
            case HintContext.Vertical: yield return vertical; break;
            case HintContext.Horizontal: yield return horizontal; break;
        }
    }

    public void ReRender()
    {
        foreach (var obj in children)
            Destroy(obj);

        Render();
    }

    private void Render()
    {
        var position = Vector3.zero;

        children = GetSprites().Select(sprite =>
        {
            var obj = new GameObject("sprite");

            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(1, 1, 1, opacity);

            obj.layer = gameObject.layer;
            obj.transform.parent = transform;
            obj.transform.localPosition = position;
            position.z -= 1;

            return obj;
        }).ToList();

        transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
    }

    void OnEnable()
    {
        Render();
    }

    void OnDisable()
    {
        foreach (var obj in children)
            Destroy(obj);
    }
}
