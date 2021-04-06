using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PressurePlate : Switch
{
    [SerializeField] GameObject cube;
    [SerializeField] MyCollider hitbox;
    [SerializeField] GameObject[] lights;
    [SerializeField] MeshRenderer platform;

    [SerializeField] Material platformMaterial;
    [SerializeField] Material activePlatformMaterial;

    [SerializeField] AudioClip DepressSound;
    [SerializeField] AudioClip ReleaseSound;

    private AudioSource audioSource;

    void Awake()
    {
        Util.GetComponent(this, out audioSource);
        hitbox.mask |= CollisionMask.Dynamic;
        StateChanged += (v) => Sync(false);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        Sync(true);
    }

    void FixedUpdate()
    {
        if (!hitbox.enabled) return;

        var overlapping = Physics.AllOverlaps(hitbox);
        IsActive = overlapping.Any();
    }

    private void Sync(bool quiet)
    {
        var size = cube.transform.localScale;
        if (IsActive)
        {
            size.y = 2 / 32f;
            if (DepressSound != null && !quiet)
                audioSource.PlayOneShot(DepressSound);
        }
        else
        {
            size.y = 6 / 32f;
            if (DepressSound != null && !quiet)
                audioSource.PlayOneShot(ReleaseSound);
        }

        cube.transform.localScale = size;

        var mats = platform.materials;
        mats[1] = IsActive
            ? activePlatformMaterial
            : platformMaterial;
        platform.materials = mats;

        foreach (var light in lights)
        {
            light.SetActive(IsActive);
        }
    }
}
