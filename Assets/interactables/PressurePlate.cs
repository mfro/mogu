using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PressurePlate : Switch
{
    [SerializeField] GameObject cube;

    [SerializeField]
    AudioClip DepressSound;
    [SerializeField]
    AudioClip ReleaseSound;
    AudioSource audioSource;

    private MyCollider physics;

    // Start is called before the first frame update
    void Start()
    {
        physics = GetComponent<MyCollider>();

        audioSource = GetComponent<AudioSource>();

        StateChanged += (v) =>
        {
            var pos = cube.transform.localScale;
            if (IsActive)
            {
                pos.y = 1 / 3f;
                if(DepressSound != null)
                    audioSource.PlayOneShot(DepressSound);
            }
            else
            {
                pos.y = 1;
                if(ReleaseSound != null)
                    audioSource.PlayOneShot(ReleaseSound);
            }

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
