using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Door : MonoBehaviour
{
    [SerializeField] Switch Switch;

    public bool IsOpen => Switch.IsActive;

    private new Renderer renderer;
    private new MyStatic collider;

    void Awake()
    {
        Util.GetComponent(this, out renderer);
        Util.GetComponent(this, out collider);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Switch == null)
        {
            print("error, switch is null");
            enabled = false;
            return;
        }

        renderer.enabled = !IsOpen;
        collider.enabled = !IsOpen;
    }

    void Update()
    {
        renderer.enabled = !IsOpen;
    }

    void FixedUpdate()
    {
        collider.enabled = !IsOpen;
    }
}
