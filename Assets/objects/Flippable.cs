using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Flippable : MonoBehaviour
{
    public event Action BeginFlip;
    public event Action<Quaternion> EndFlip;

    [NonSerialized] public bool flipping = false;
    [NonSerialized] public Vector2 down;

    private Vector3 scaleSave;
    private bool snapPosition;

    // Start is called before the first frame update
    void Awake()
    {
        scaleSave = transform.localScale;
        down = Util.Round(transform.rotation * Vector2.down);
    }


#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        var dyn = GetComponent<MyDynamic>();
        if (dyn?.gravity != true) return;

        var down = Util.Round(transform.rotation * Vector3.down);
        var o = transform.position;
        var points = new Vector2[] {
            new Vector2(-2, 12),
            new Vector2(2, 12),
            new Vector2(2, -2),
            new Vector2(8, -2),
            new Vector2(0, -12),
            new Vector2(-8, -2),
            new Vector2(-2, -2),
            new Vector2(-2, 12),
        };

        var color = Color.black;
        color.a = 0.5f;
        Gizmos.color = color;

        for (var i = 1; i < points.Length; ++i)
        {
            var p1 = o + transform.rotation * points[i - 1] / 32;
            var p2 = o + transform.rotation * points[i] / 32;

            p1.z = -1;
            p2.z = -1;

            Gizmos.DrawLine(p1, p2);
        }
    }
#endif

    private List<MyCollider> disabled;

    public void DoBeginFlip()
    {
        flipping = true;

        snapPosition = transform.localPosition.x % 0.5f == 0
            && transform.localPosition.y % 0.5f == 0
            && transform.localPosition.z % 0.5f == 0;

        disabled = GetComponentsInChildren<MyCollider>()
            .Where(p => p.enabled)
            .ToList();

        foreach (var physics in disabled)
            physics.enabled = false;

        BeginFlip?.Invoke();
    }

    public void DoEndFlip(Quaternion delta)
    {
        down = Util.Round(delta * down);
        flipping = false;

        transform.localScale = scaleSave;
        transform.localRotation = Quaternion.Euler(Util.Round(transform.localRotation.eulerAngles));

        if (snapPosition)
        {
            transform.localPosition = Util.Round(transform.localPosition * 2) / 2;
        }
        else
        {
            var pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }

        foreach (var physics in disabled)
            physics.enabled = true;

        disabled = null;

        EndFlip?.Invoke(delta);
    }
}
