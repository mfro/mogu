using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Start is called before the first frame update

    public bool shaking = false;

    [SerializeField] float duration, strength, decay;

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

    private async void Shake(float strength)
    {
        shaking = true;
        Vector3 startPosition = transform.position;
        var player = FindObjectOfType<PlayerController>();
        var orientation = Quaternion.FromToRotation(Vector2.down, player.flip.down);

        for (var i = 0; i < offsets.Length; ++i)
        {
            transform.position = startPosition + orientation * offsets[i] / 32;

            await Util.NextFixedUpdate();

            strength *= decay;
        }

        transform.position = startPosition;
        shaking = false;
    }

    public void DoShake() => DoShake(strength);
    public void DoShake(float strength)
    {
        if (shaking) return;
        Shake(strength);
    }
}
