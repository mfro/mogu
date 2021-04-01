using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipError : MonoBehaviour
{
    [SerializeField] public float duration;

    [NonSerialized] public float startTime;

    private SpriteRenderer sprite;

    private Color startColor;
    private Color endColor;

    void Awake()
    {
        Util.GetComponent(this, out sprite);
    }

    void Start()
    {
        startColor = sprite.color;
        endColor = sprite.color;
        endColor.a = 0;
    }

    // Update is called once per frame
    void Update()
    {
        var progress = (Time.time - startTime) / duration;
        if (progress >= 1)
        {
            Destroy(gameObject);
        }
        else
        {
            var faded = Color.Lerp(startColor, endColor, progress);
            sprite.color = faded;
        }
    }
}
