using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Door : MonoBehaviour
{
    [SerializeField] Switch mySwitchObject;

    public bool IsOpen => mySwitchObject.IsActive;

    private MyCollider physics;
    private Flippable flippable;
    public new Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        if (mySwitchObject == null)
        {
            print("error, switch is null");
            enabled = false;
            return;
        }

        flippable = GetComponent<Flippable>();
        renderer = GetComponent<Renderer>();
        physics = GetComponent<MyCollider>();
    }

    void Update()
    {
        renderer.enabled = !IsOpen;
    }
}
