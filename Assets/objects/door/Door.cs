using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Switch Switch;
    [SerializeField] Material open;
    [SerializeField] Material closed;

    [SerializeField] MyStatic hitbox;
    [SerializeField] MeshRenderer cube;

    public bool IsOpen => Switch.IsActive;

    // Start is called before the first frame update
    void Start()
    {
        if (Switch == null)
        {
            print("error, switch is null");
            enabled = false;
            return;
        }

        cube.material = IsOpen ? open : closed;
    }

    void Update()
    {
        cube.material = IsOpen ? open : closed;
    }

    void FixedUpdate()
    {
        hitbox.enabled = !IsOpen;
    }
}
