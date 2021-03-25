using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PressurePlate : Switch
{
    private int numObjectsPressing = 0;

    [SerializeField] GameObject cube;

    [SerializeField] float animDuration;
    [SerializeField] string[] interactibleLayers;

    private Flippable flippable;
    private MyCollider physics;

    // Start is called before the first frame update
    void Start()
    {
        flippable = GetComponent<Flippable>();
        physics = GetComponent<MyCollider>();

        StateChanged += (v) =>
        {
            var pos = cube.transform.localScale;
            if (IsActive)
                pos.y = 1 / 3f;
            else
                pos.y = 1;

            cube.transform.localScale = pos;
        };
    }

    void Update()
    {
        if (!physics.enabled) return;

        var overlapping = Physics.AllOverlaps(physics, CollideReason.PressurePlate);
        IsActive = overlapping.Any();
    }
}
