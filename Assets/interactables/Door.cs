using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [SerializeField] Switch mySwitchObject;

    private Flippable flippable;
    private Collider2D col;
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
        col = GetComponent<Collider2D>();
        renderer = GetComponent<Renderer>();

        mySwitchObject.StateChanged += OnSwitchChange;

        flippable.EndFlip += () =>
        {
            renderer.enabled = !mySwitchObject.IsActive;
            col.isTrigger = mySwitchObject.IsActive;
        };
    }

    private void LateUpdate()
    {
        renderer.enabled = !doorShut;
        col.isTrigger = doorShut;
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
