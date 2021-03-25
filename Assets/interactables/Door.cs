using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Door : MonoBehaviour
{
    [SerializeField] Switch mySwitchObject;

    public bool IsOpen => !mySwitchObject.IsActive;

    private MyCollider physics;
    private Flippable flippable;
    private new Renderer renderer;

    private bool doorShut;

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

        mySwitchObject.StateChanged += OnSwitchChange;
    }

    void Update()
    {
        renderer.enabled = IsOpen;
        // physics.enabled = IsActive;
    }

    private void LateUpdate()
    {
        renderer.enabled = !doorShut;
    }

    private void OnSwitchChange(bool active)
    {
        doorShut = active;
    }

    private void OnDestroy()
    {
        mySwitchObject.StateChanged -= OnSwitchChange;
    }
}
