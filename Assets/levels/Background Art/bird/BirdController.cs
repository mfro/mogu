using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public Vector2 velocity;

    private float remaining;

    void Awake()
    {
        remaining = 120;
        Parallax.parallax += OnParallax;
    }

    void OnDestroy()
    {
        Parallax.parallax -= OnParallax;
    }

    void FixedUpdate()
    {
        if (remaining <= 0)
        {
            Destroy(this);
        }
        else if (Physics.IsEnabled)
        {
            transform.position += new Vector3(velocity.x * Time.fixedDeltaTime, velocity.y * Time.fixedDeltaTime, 0);
            remaining -= Time.fixedDeltaTime;
        }

        GetComponent<Animator>().enabled = Physics.IsEnabled;
    }

    private void OnParallax(Vector2 delta)
    {
        transform.position += (Vector3)(delta * 0.6f);
    }
}
