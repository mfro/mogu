using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PressurePlate : Switch
{
    [SerializeField] GameObject cube;

    private MyCollider physics;

    // Start is called before the first frame update
    void Start()
    {
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

    void FixedUpdate()
    {
        if (!physics.enabled) return;

        var overlapping = Physics.AllOverlaps(CollisionMask.Dynamic, physics);
        IsActive = overlapping.Any();
    }
}
