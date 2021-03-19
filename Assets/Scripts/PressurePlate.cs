using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : Switch
{
    private int numObjectsPressing = 0;

    [SerializeField] GameObject cube;

    [SerializeField] float animDuration;
    [SerializeField] string[] interactibleLayers;

    // Start is called before the first frame update
    void Start()
    {
        LayerMask layermask = LayerMask.GetMask(interactibleLayers);
        numObjectsPressing = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0, layermask).Length - 1;
        //print("Plate initialized with this many objects pressing me: " + numObjectsPressing);

        StateChanged += (v) =>
        {
            var pos = cube.transform.position;
            if (IsActive)
                pos.y -= 2 / 32f;
            else
                pos.y += 2 / 32f;

            cube.transform.position = pos;
        };
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        numObjectsPressing++;

        IsActive = numObjectsPressing != 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (numObjectsPressing > 0)
            numObjectsPressing--;

        IsActive = numObjectsPressing != 0;
    }
}
