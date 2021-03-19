using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour, iSwitch
{
    public event Action switchEnabled;
    public event Action switchDisabled;

    private int numObjectsPressing = 0;

    [SerializeField] String[] interactibleLayers;

    [SerializeField] float animDuration;

    private Flippable flippable;

    // Start is called before the first frame update
    void Start()
    {

        LayerMask layermask = LayerMask.GetMask(interactibleLayers);
        numObjectsPressing = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0, layermask).Length - 1;
        //print("Plate initialized with this many objects pressing me: " + numObjectsPressing);

        if (numObjectsPressing > 0) switchEnabled?.Invoke();

        flippable = GetComponent<Flippable>();

        if (flippable != null)
        {
            //flippable.BeginFlip += () =>
            //{

            //};

            flippable.EndFlip += () =>
            {
                var pos = transform.position;
                pos.z = 0;
                transform.position = pos;
            };
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        numObjectsPressing++;

        if (numObjectsPressing > 0) switchEnabled?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        numObjectsPressing--;
        numObjectsPressing = Mathf.Max(numObjectsPressing, 0);

        if (numObjectsPressing == 0) switchDisabled?.Invoke();
    }

}
