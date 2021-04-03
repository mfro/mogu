using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MyCollider : MonoBehaviour
{
    public Vector2 scale = new Vector2(1, 1);
    public Vector2 offset;

    [NonSerialized]
    public CollisionMask mask = CollisionMask.None;

    [NonSerialized]
    public Vector2 position;

    [NonSerialized]
    public HashSet<MyCollider> touching = new HashSet<MyCollider>();

    [NonSerialized]
    public Flippable flip;

    public Rect bounds => GetBounds(position);

    void OnEnable()
    {
        flip = GetComponentInParent<Flippable>();
        position = Physics.FromUnity(transform.position);

        if (GetComponent<Flippable>() != null)
        {
            mask |= CollisionMask.Flipping;
        }

        if (flip != null)
        {
            flip.EndFlip += (delta) =>
            {
                position = Physics.FromUnity(transform.position);
            };
        }

        Physics.Enable(this);
    }

    void OnDisable()
    {
        Physics.Disable(this);
    }

    public void UpdatePosition()
    {
        transform.position = Physics.ToUnity(position);
    }

    private Rect GetBounds(Vector2 position)
    {
        var fullScale = Util.Round(transform.lossyScale * scale * 1e5f) / 1e5f;
        if (fullScale.x % (1 / 32f) != 0) throw new Exception($"invalid scale on collider: {fullScale.x}");
        if (fullScale.y % (1 / 32f) != 0) throw new Exception($"invalid scale on collider: {fullScale.y}");

        var o = (Vector2)Util.Round(transform.rotation * offset);
        var s = Physics.FromUnity(transform.rotation * (transform.lossyScale * scale));

        s = Util.Round(s);
        s.x = Math.Abs(s.x);
        s.y = Math.Abs(s.y);

        return Util.RectFromCenterSize(position + o, s);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (!isActiveAndEnabled) return;

        try
        {
            GetBounds(Physics.FromUnity(transform.position));
        }
        catch (Exception e)
        {
            print(e.Message);
            var color = Color.red;
            Gizmos.color = color;
            Gizmos.DrawCube(transform.position, new Vector3(2, 2, 2));
            return;
        }

        if (Selection.activeTransform == null)
            return;

        var isChild = Selection.transforms.Any(c => transform.IsChildOf(c));
        var isParent = Selection.transforms.Any(c => c.IsChildOf(transform));

        if (isChild)
        {
            var bounds = GetBounds(Physics.FromUnity(transform.position));

            var color = Color.green;
            Gizmos.color = color;
            Gizmos.DrawWireCube(Physics.ToUnity(bounds.center), Physics.ToUnity(bounds.size));
        }
    }
#endif
}
