using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyDynamic : MyCollider
{
    public bool gravity;
    public float pushRatio;

    public Vector2 velocity;

    public Vector2 down => flip?.down ?? Vector2.down;

    private bool? _grounded;
    public bool grounded => _grounded ??
        (_grounded = Physics.AllCollisions(bounds.Shift(down), mask).Where(c => c.Item1 != this).Any()).Value;

    private bool? _ceilinged;
    public bool ceilinged => _ceilinged ??
        (_ceilinged = Physics.AllCollisions(bounds.Shift(-down), mask).Where(c => c.Item1 != this).Any()).Value;

    [NonSerialized]
    public Vector2 remainder;

    void Awake()
    {
        mask |= CollisionMask.Physical;
        mask |= CollisionMask.Dynamic;
    }

    void Start()
    {
        if (flip != null) flip.EndFlip += (delta) =>
        {
            velocity = Vector2.zero;
        };
    }

    void FixedUpdate()
    {
        if (!IsEnabled) return;

        if (gravity)
        {
            velocity += flip.down * Physics.GRAVITY * Time.fixedDeltaTime;
            velocity.y = Mathf.Clamp(velocity.y, -Physics.MAX_FALL_SPEED, Physics.MAX_FALL_SPEED);
            velocity.x = Mathf.Clamp(velocity.x, -Physics.MAX_FALL_SPEED, Physics.MAX_FALL_SPEED);
        }

        if (velocity != Vector2.zero)
        {
            Physics.Move(this, velocity);

            if (velocity.x != 0 && !Physics.CanMove(this, new Vector2(Mathf.Sign(velocity.x), 0)))
                velocity.x = 0;

            if (velocity.y != 0 && !Physics.CanMove(this, new Vector2(0, Mathf.Sign(velocity.y))))
                velocity.y = 0;

            UpdatePosition();
        }

        _grounded = null;
        _ceilinged = null;
    }
}
