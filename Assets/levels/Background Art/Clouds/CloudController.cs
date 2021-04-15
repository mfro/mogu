using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{

    [SerializeField] float amplitude, frequency;

    Vector3 origPos;

    void Awake()
    {
        Parallax.parallax += OnParallax;

        origPos = transform.position;
    }

    void OnDestroy()
    {
        Parallax.parallax -= OnParallax;
    }

    private void OnParallax(Vector2 delta)
    {
        transform.position += (Vector3)(delta * 0.8f);
        origPos += (Vector3)(delta * 0.8f);
    }

    private float x = 0f;
    private void FixedUpdate()
    {
        x += Time.fixedDeltaTime;

        
        transform.position = new Vector3(origPos.x, origPos.y + amplitude * Mathf.Sin(2 * Mathf.PI * x * frequency), origPos.z);
    }
}
