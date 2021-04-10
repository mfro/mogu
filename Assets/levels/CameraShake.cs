using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool shaking = false;

    public static CameraShake instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        shaking = false;
    }

    public static Vector3[] FlipError = {
        new Vector2(1, 0),
        new Vector2(1, 0),
        new Vector2(0, 0),
        new Vector2(0, 0),
        new Vector2(-1, 0),
        new Vector2(-1, 0),
    };

    public static Vector3[] Death = {
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
        new Vector2(1, 0),
        new Vector2(0, 0),
        new Vector2(-1, 0),
        new Vector2(-1, 0),
        new Vector2(0, 0),
    };

    public async void DoShake(Vector3[] timing)
    {
        if (shaking) return;
        shaking = true;
        Vector3 startPosition = transform.position;
        var player = FindObjectOfType<PlayerController>();
        var orientation = Quaternion.FromToRotation(Vector2.down, player.flip.down);

        for (var i = 0; i < timing.Length; ++i)
        {
            transform.position = startPosition + orientation * timing[i] / 32;

            await Util.NextFixedUpdate();
        }

        transform.position = startPosition;
        shaking = false;
    }
}
