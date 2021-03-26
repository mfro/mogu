using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MyCollider : MonoBehaviour
{
    public bool gravity;
    public Vector2 size;
    public Vector2 offset;
    public float pushRatio;

    public bool grounded;
    public bool ceilinged;
    public Vector2 velocity;
    [NonSerialized]
    public Vector2 position;
    public Vector2 remainder;

    private Flippable flip;

    public Rect bounds => GetBounds(position);

    void OnEnable()
    {
        position = Physics.FromUnity(transform.position);
        flip = GetComponent<Flippable>();
        Physics.allColliders.Add(this);

        if (flip != null) flip.EndFlip += (delta) =>
        {
            position = Physics.FromUnity(transform.position);
            velocity = Vector2.zero;
        };
    }

    void OnDisable()
    {
        Physics.allColliders.Remove(this);
    }

    void FixedUpdate()
    {
        if (flip?.flipping == true)
            return;

        if (gravity)
        {
            velocity += flip.down * Physics.GRAVITY * Time.fixedDeltaTime;
            velocity.y = Mathf.Clamp(velocity.y, -Physics.MAX_FALL_SPEED, Physics.MAX_FALL_SPEED);
            velocity.x = Mathf.Clamp(velocity.x, -Physics.MAX_FALL_SPEED, Physics.MAX_FALL_SPEED);
        }

        if (velocity != Vector2.zero)
        {
            Physics.Move(this, velocity);

            var down = flip?.down ?? Vector2.down;

            var under = Physics.AllOverlaps(bounds.Shift(down), CollideReason.Collision).Where(c => c.Item1 != this);
            grounded = under.Any();

            var above = Physics.AllOverlaps(bounds.Shift(-down), CollideReason.Collision).Where(c => c.Item1 != this);
            ceilinged = above.Any();

            if (velocity.x != 0 && !Physics.CanMove(this, new Vector2(Mathf.Sign(velocity.x), 0)))
                velocity.x = 0;

            if (velocity.y != 0 && !Physics.CanMove(this, new Vector2(0, Mathf.Sign(velocity.y))))
                velocity.y = 0;

            UpdatePosition();
        }
    }

    public void UpdatePosition()
    {
        transform.position = Physics.ToUnity(position);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (Selection.activeTransform == null)
            return;

        var isChild = transform.IsChildOf(Selection.activeTransform);
        var isParent = Selection.activeTransform.IsChildOf(transform);

        if (isChild || isParent)
        {
            flip = GetComponent<Flippable>();
            var bounds = GetBounds(Physics.FromUnity(transform.position));

            var color = Color.green;
            Gizmos.color = color;
            Gizmos.DrawWireCube(Physics.ToUnity(bounds.center), Physics.ToUnity(bounds.size));
        }
    }
#endif

    private Rect GetBounds(Vector2 position)
    {
        var o = offset;
        var s = size;

        if (flip != null)
        {
            o = Quaternion.FromToRotation(Vector2.down, flip.down) * o;
            if (flip.down.x != 0) s = new Vector2(s.y, s.x);
        }

        return Physics.RectFromCenterSize(position + o, s);
    }
}
