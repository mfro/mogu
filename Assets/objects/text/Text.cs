using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum TextAlignment
{
    Left,
    Center,
    Right,
}

[ExecuteAlways]
public class Text : MonoBehaviour
{
    [SerializeField]
    private string _text;

    public Texture2D font;
    public string text
    {
        get => _text;
        set { _text = value; ReDraw(); }
    }

    public Color color = Color.black;
    public TextAlignment alignment = TextAlignment.Left;

    private const string UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LOWER = "abcdefghijklmnopqrstuvwxyz";
    private const string NUMBER = "0123456789.,:/-!";
    private static Sprite[] upper;
    private static Sprite[] lower;
    private static Sprite[] number;

    private static Dictionary<char, int> kerning = new Dictionary<char, int>
    {
        ['f'] = 4,
        ['i'] = 3,
        ['j'] = 3,
        ['k'] = 4,
        ['l'] = 3,
        ['r'] = 4,
        ['t'] = 3,
        ['.'] = 1,
        [','] = 2,
        [':'] = 1,
        ['/'] = 3,
        ['!'] = 1,
    };

    private List<GameObject> children;
    private void Draw()
    {
        if (number == null || number[0] == null)
        {
            number = new Sprite[NUMBER.Length];
            for (var i = 0; i < NUMBER.Length; ++i)
            {
                number[i] = Sprite.Create(font, new Rect(i * 5, 0, 5, 9), new Vector2(0, 0), 32, 0, SpriteMeshType.Tight, Vector4.zero, false);
            }

            upper = new Sprite[UPPER.Length];
            lower = new Sprite[LOWER.Length];
            for (var i = 0; i < UPPER.Length; ++i)
            {
                lower[i] = Sprite.Create(font, new Rect(i * 5, 9, 5, 9), new Vector2(0, 0), 32, 0, SpriteMeshType.Tight, Vector4.zero, false);
                upper[i] = Sprite.Create(font, new Rect(i * 5, 18, 5, 9), new Vector2(0, 0), 32, 0, SpriteMeshType.Tight, Vector4.zero, false);
            }
        }

        var ui = GetComponent<RectTransform>();

        var x = -1;
        var layout = text.Select(ch =>
        {
            var pos = x + 1;
            if (ch == ' ')
                x += 3;
            else
                x += kerning.ContainsKey(ch)
                    ? kerning[ch] + 1
                    : 6;

            return (ch, pos);
        }).ToList();

        if (x < 0) x = 0;

        float origin;
        if (alignment == TextAlignment.Left)
            origin = ui != null ? ui.rect.xMin : 0;
        else if (alignment == TextAlignment.Center)
            origin = ui != null ? ui.rect.center.x - x / 2 : -x / 2;
        else if (alignment == TextAlignment.Right)
            origin = ui != null ? ui.rect.xMax - x : -x;
        else
            throw new Exception("invalid alignment");

        children = layout.Select(pair =>
        {
            var sprite = UPPER.Contains(pair.ch) ? upper[UPPER.IndexOf(pair.ch)]
                : LOWER.Contains(pair.ch) ? lower[LOWER.IndexOf(pair.ch)]
                : NUMBER.Contains(pair.ch) ? number[NUMBER.IndexOf(pair.ch)]
                : null;

            if (sprite == null) return null;

            var obj = new GameObject("char");
            obj.transform.SetParent(transform, false);
            obj.layer = gameObject.layer;
            obj.hideFlags |= HideFlags.DontSave;

            if (ui != null)
            {
                var rect = obj.AddComponent<RectTransform>();
                rect.offsetMax = new Vector2(5, 9);
                rect.offsetMin = new Vector2(0, 0);
                // rect.anchorMin = new Vector2(x, -2);
                // rect.anchorMax = new Vector2(x + 5, 7);

                obj.transform.localPosition = new Vector3(origin + pair.pos + 2.5f, -2, 0);

                var renderer = obj.AddComponent<Image>();
                renderer.sprite = sprite;
                renderer.color = color;
            }
            else
            {
                obj.transform.localPosition = new Vector3(origin + pair.pos / 32f, -2 / 32f, 0);

                var renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.color = color;
            }

            return obj;
        }).ToList();
    }

    private void Clean()
    {
        if (children != null)
        {
            foreach (var child in children)
            {
                DestroyImmediate(child);
            }
            children = null;
        }
    }

    public void ReDraw()
    {
        if (!enabled) return;

        Clean();
        Draw();
    }

    void OnEnable()
    {
        Clean();
        Draw();
    }

    void OnDisable()
    {
        Clean();
    }
}
