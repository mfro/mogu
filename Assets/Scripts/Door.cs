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
            this.enabled = false;
            return;
        }

        flippable = GetComponent<Flippable>();
        col = GetComponent<Collider2D>();
        renderer = GetComponent<Renderer>();

        if (flippable != null)
        {

            flippable.BeginFlip += () =>
            {
            };

            flippable.EndFlip += () =>
            {
            };
        }

        mySwitchObject.StateChanged += OnSwitchChange;
    }

    private void OnSwitchChange(bool active)
    {
        if (flippable.flipping) return;
        renderer.enabled = !active;
        col.isTrigger = active;
    }

    private void OnDestroy()
    {
        mySwitchObject.StateChanged -= OnSwitchChange;
    }
}
