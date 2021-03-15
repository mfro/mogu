using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flippable : MonoBehaviour
{
    public event Action BeginFlip;
    public event Action EndFlip;
    public bool flipping = false;
    public Vector2 down = Vector2.down;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DoBeginFlip()
    {
        BeginFlip?.Invoke();
        flipping = true;
    }

    public void DoEndFlip(Quaternion delta)
    {
        down = delta * down;
        down.x = Mathf.Round(down.x);
        down.y = Mathf.Round(down.y);
        flipping = false;
        EndFlip?.Invoke();
    }
}
