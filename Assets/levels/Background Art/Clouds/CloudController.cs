using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    void Awake()
    {
        Parallax.parallax += OnParallax;
    }

    void OnDestroy()
    {
        Parallax.parallax -= OnParallax;
    }

    private void OnParallax(Vector2 delta)
    {
        transform.position += (Vector3)(delta * 0.8f);
    }
}
