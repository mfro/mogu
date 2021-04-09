using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    public bool reverse = false;

    private const string UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LOWER = "abcdefghijklmnopqrstuvwxyz";
    private const string NUMBER = "0123456789.";
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
    };

    private List<GameObject> children;
    private void Draw()
    {
        var isUI = GetComponent<RectTransform>() != null;

        var x = reverse ? 1 : -1;
        foreach (var ch in reverse ? text.Reverse() : text)
        {
            if (ch == ' ')
            {
                x += reverse ? -3 : 3;
            }
            else
            {
                if (reverse)
                {
                    x -= kerning.ContainsKey(ch)
                        ? kerning[ch] + 1
                        : 6;
                }
                else
                {
                    x += 1;
                }

                var sprite = UPPER.Contains(ch) ? upper[UPPER.IndexOf(ch)]
                    : LOWER.Contains(ch) ? lower[LOWER.IndexOf(ch)]
                    : NUMBER.Contains(ch) ? number[NUMBER.IndexOf(ch)]
                    : null;

                if (sprite == null) continue;

                var obj = new GameObject("char");
                obj.transform.SetParent(transform, false);
                obj.layer = gameObject.layer;

                if (isUI)
                {
                    var rect = obj.AddComponent<RectTransform>();
                    rect.offsetMax = new Vector2(5, 9);
                    rect.offsetMin = new Vector2(0, 0);
                    // rect.anchorMin = new Vector2(x, -2);
                    // rect.anchorMax = new Vector2(x + 5, 7);

                    obj.transform.localPosition = new Vector3(x + 2.5f, -6.5f, 0);

                    var renderer = obj.AddComponent<Image>();
                    renderer.sprite = sprite;
                    renderer.color = color;
                }
                else
                {
                    obj.transform.localPosition = new Vector3(x / 32f, -2 / 32f, 0);

                    var renderer = obj.AddComponent<SpriteRenderer>();
                    renderer.sprite = sprite;
                    renderer.color = color;
                }

                if (!reverse)
                {
                    x += kerning.ContainsKey(ch)
                        ? kerning[ch]
                        : 5;
                }

                children.Add(obj);
            }
        }
    }

    public void ReDraw()
    {
        foreach (var child in children)
            Destroy(child);
        children = new List<GameObject>();

        Draw();
    }

    void Awake()
    {
        if (number == null)
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

        children = new List<GameObject>();
        Draw();
    }
}
