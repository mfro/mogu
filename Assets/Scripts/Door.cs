using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{

    [SerializeField] GameObject mySwitchObject;

    private Flippable flippable;
    private Collider2D col;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Renderer renderer;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    private iSwitch mySwitch;

    private bool doorShut;

    private bool doorCanToggle;

    // Start is called before the first frame update
    void Start()
    {
        doorCanToggle = true;

        mySwitch = mySwitchObject.GetComponent<iSwitch>();

        if (mySwitch == null)
        {
            print("error, switch is null");
            this.enabled = false;
            return;
        }

        flippable = GetComponent<Flippable>();
        col = GetComponent<Collider2D>();
        renderer = GetComponent<Renderer>();

        if (flippable != null )
        {

            flippable.BeginFlip += () =>
            {

            };

            flippable.EndFlip += () =>
            {
                var pos = transform.position;
                pos.z = 0;
                transform.position = pos;
            };
        }



        mySwitch.switchDisabled += CloseDoor;
        mySwitch.switchEnabled += OpenDoor;
    }

    private void CloseDoor()
    {
        renderer.enabled = true;
        col.isTrigger = false;
    }

    private void OpenDoor()
    {

        renderer.enabled = false;
        col.isTrigger = true;
    }

    private void OnDestroy()
    {
        mySwitch.switchEnabled -= OpenDoor;
        mySwitch.switchDisabled -= CloseDoor;
    }
}
