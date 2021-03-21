﻿using System;
using UnityEngine;

public class Flippable : MonoBehaviour
{
    public event Action BeginFlip;
    public event Action EndFlip;
    public bool flipping = false;
    public Vector2 down = Vector2.down;

    private Vector3 scaleSave;
    private bool snapPosition;

    private new Collider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        scaleSave = transform.localScale;

        collider = GetComponent<Collider2D>();
    }

    public void DoBeginFlip()
    {
        flipping = true;

        snapPosition = transform.localPosition.x % 0.5f == 0
            && transform.localPosition.x % 0.5f == 0
            && transform.localPosition.x % 0.5f == 0;

        if (collider != null) collider.enabled = false;

        BeginFlip?.Invoke();
    }

    public void DoEndFlip(Quaternion delta)
    {
        down = delta * down;
        down.x = Mathf.Round(down.x);
        down.y = Mathf.Round(down.y);
        flipping = false;

        if (collider != null) collider.enabled = true;

        transform.localScale = scaleSave;
        var angles = transform.localRotation.eulerAngles;
        angles.x = Mathf.Round(angles.x);
        angles.y = Mathf.Round(angles.y);
        angles.z = Mathf.Round(angles.z);
        transform.localRotation = Quaternion.Euler(angles);

        if (snapPosition)
        {
            var pos = transform.localPosition * 2;
            pos.x = Mathf.Round(pos.x);
            pos.y = Mathf.Round(pos.y);
            pos.z = Mathf.Round(pos.z);
            transform.localPosition = pos / 2;
        }
        else
        {
            var pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }

        EndFlip?.Invoke();
    }
}