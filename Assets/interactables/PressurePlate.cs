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
    private EdgeCollider2D edgeCollider;

    // Start is called before the first frame update
    void Start()
    {
        flippable = GetComponent<Flippable>();
        edgeCollider = GetComponent<EdgeCollider2D>();

        LayerMask layermask = LayerMask.GetMask(interactibleLayers);
        numObjectsPressing = Physics2D.OverlapBoxAll(transform.position, transform.lossyScale, 0, layermask)
            .Count(o => o.gameObject != gameObject);
        // print("Plate initialized with this many objects pressing me: " + numObjectsPressing);

        StateChanged += (v) =>
        {
            var pos = cube.transform.localScale;
            if (IsActive)
                pos.y = 1 / 3f;
            else
                pos.y = 1;

            cube.transform.localScale = pos;
        };

        flippable.EndFlip += async () =>
        {
            await Task.Yield();
            IsActive = numObjectsPressing != 0;
        };
    }


    private void Update()
    {

        if (flippable.flipping) return;

        LayerMask layermask = LayerMask.GetMask(interactibleLayers);
        numObjectsPressing = Physics2D.OverlapBoxAll(transform.position, transform.lossyScale, 0, layermask)
            .Count(o => o.gameObject != gameObject);

        IsActive = numObjectsPressing != 0;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    numObjectsPressing++;

    //    if (flippable.flipping) return;

    //    IsActive = numObjectsPressing != 0;
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (numObjectsPressing > 0)
    //        numObjectsPressing--;

    //    if (flippable.flipping) return;

    //    IsActive = numObjectsPressing != 0;
    //}
}
