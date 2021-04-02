using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipError : MonoBehaviour
{
    [SerializeField] public float duration;

    private SpriteRenderer sprite;

    private Color startColor;
    private Color endColor;
    private float remaining;

    void Awake()
    {
        Util.GetComponent(this, out sprite);
    }

    void Start()
    {
        remaining = duration;
        startColor = sprite.color;
        endColor = sprite.color;
        endColor.a = 0;
    }

    // Update is called once per frame
    void Update()
    {
        var progress = 1 - remaining / duration;
        var faded = Color.Lerp(startColor, endColor, progress);
        sprite.color = faded;
    }

    void FixedUpdate()
    {
        if (remaining >= Time.fixedDeltaTime)
        {
            remaining -= Time.fixedDeltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
