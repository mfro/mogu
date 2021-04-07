using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Text : MonoBehaviour
{
    public Texture2D font;
    public string text;

    private const string UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LOWER = "abcdefghijklmnopqrstuvwxyz";
    private const string NUMBER = "0123456789";
    private Sprite[] upper;
    private Sprite[] lower;
    private Sprite[] number;

    private static Dictionary<char, int> kerning = new Dictionary<char, int>
    {
        ['f'] = 4,
        ['i'] = 3,
        ['j'] = 3,
        ['k'] = 4,
        ['l'] = 3,
        ['r'] = 4,
        ['t'] = 3,
    };

    void Awake()
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

        var x = 0;
        foreach (var ch in text)
        {
            if (ch == ' ')
            {
                x += 3;
            }
            else
            {
                var sprite = UPPER.Contains(ch) ? upper[UPPER.IndexOf(ch)]
                    : LOWER.Contains(ch) ? lower[LOWER.IndexOf(ch)]
                    : NUMBER.Contains(ch) ? number[NUMBER.IndexOf(ch)]
                    : null;

                if (sprite == null) continue;

                var obj = new GameObject("char");
                obj.transform.SetParent(transform, false);
                obj.transform.localPosition = new Vector3(x / 32f, -2 / 32f, 0);

                var renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;

                x += kerning.ContainsKey(ch)
                    ? kerning[ch] + 1
                    : 6;
            }
        }
    }
}
