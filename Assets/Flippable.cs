using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flippable : MonoBehaviour
{
    public event Action Flip;
    public bool flipping = false;
    public Vector3 down = Vector3.down;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDown(Vector2 down)
    {
        this.down = down;
        Flip?.Invoke();
    }
}
