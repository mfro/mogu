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
    public Vector2 position;

    [NonSerialized]
    public CollisionMask mask = CollisionMask.None;

    [NonSerialized]
    public Flippable flip;

    public Rect bounds => GetBounds(position);

    void OnEnable()
    {
        position = Physics.FromUnity(transform.position);
        flip = GetComponentInParent<Flippable>();

        if (flip != null)
        {
            mask |= CollisionMask.Flipping;

            flip.EndFlip += (delta) =>
            {
                position = Physics.FromUnity(transform.position);
            };
        }

        Physics.allColliders.Add(this);
    }

    void OnDisable()
    {
        Physics.allColliders.Remove(this);
    }

    public void UpdatePosition()
    {
        transform.position = Physics.ToUnity(position);
    }

    private Rect GetBounds(Vector2 position)
    {
        var fullScale = Physics.Round(transform.lossyScale * scale * 1e5f) / 1e5f;
        if (fullScale.x % (1 / 32f) != 0) throw new Exception($"invalid scale on collider: {fullScale.x}");
        if (fullScale.y % (1 / 32f) != 0) throw new Exception($"invalid scale on collider: {fullScale.y}");

        var o = offset;
        var s = Physics.FromUnity(transform.lossyScale * scale);

        if (flip != null)
        {
            o = Quaternion.FromToRotation(Vector2.down, flip.down) * o;
            if (flip.down.x != 0) s = new Vector2(s.y, s.x);
        }

        return Physics.RectFromCenterSize(position + o, s);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
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

        var isChild = transform.IsChildOf(Selection.activeTransform);
        var isParent = Selection.activeTransform.IsChildOf(transform);

        if (isChild || isParent)
        {
            flip = GetComponentInParent<Flippable>();
            var bounds = GetBounds(Physics.FromUnity(transform.position));

            var color = Color.green;
            Gizmos.color = color;
            Gizmos.DrawWireCube(Physics.ToUnity(bounds.center), Physics.ToUnity(bounds.size));
        }
    }
#endif
}
