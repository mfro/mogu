using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool shaking = false;

    public static CameraShake cameraShake;

    void Awake()
    {
        if (cameraShake == null)
        {
            cameraShake = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        shaking = false;
    }

    private static Vector3[] offsets = {
        new Vector2(1, 0),
        // new Vector2(2, 0),
        // new Vector2(2, 0),
        new Vector2(1, 0),
        new Vector2(0, 0),
        new Vector2(-1, 0),
        // new Vector2(-2, 0),
        // new Vector2(-2, 0),
        new Vector2(-1, 0),
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(0, 0),
        new Vector2(-1, 0),
        new Vector2(0, 0),
    };

    public async void DoShake()
    {
        if (shaking) return;
        shaking = true;
        Vector3 startPosition = transform.position;
        var player = FindObjectOfType<PlayerController>();
        var orientation = Quaternion.FromToRotation(Vector2.down, player.flip.down);

        for (var i = 0; i < offsets.Length; ++i)
        {
            transform.position = startPosition + orientation * offsets[i] / 32;

            await Util.NextFixedUpdate();
        }

        transform.position = startPosition;
        shaking = false;
    }
}
