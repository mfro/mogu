using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Door : MonoBehaviour
{
    [SerializeField] Switch Switch;
    [SerializeField] Material open;
    [SerializeField] Material closed;

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

        renderer.material = IsOpen ? open : closed;
    }

    void Update()
    {
        renderer.material = IsOpen ? open : closed;
    }

    void FixedUpdate()
    {
        collider.enabled = !IsOpen;
    }
}
