﻿using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float MAX_FALL_SPEED = 320f / 8;
    public float RUNNING_SPEED_LIMIT = 90f / 8;
    public float GROUND_RUNNING_ACCELERATION = 3000f / 8;
    public float GROUND_DECELERATION = 400f;
    public float AIR_RUNNING_ACCELERATION = 1800f / 8;
    public float AIR_DECELERATION = 320f;
    public float JUMP_TIME = 0.3f;
    public float JUMP_HEIGHT = 3.2f;

    public float MAX_JUMP_TIME = 0.20f;
    public float MAX_JUMP_HEIGHT = 3.0f;

    public bool grounded;
    public bool ceilinged;
    public Vector2 velocity;

    public bool jumping = false;
    public float jump_end = 0;
    public float jump_progress = 0;

    Vector2 movement_input;
    bool jumping_input;

    private Rigidbody2D body;
    private new BoxCollider2D collider;
    private Flippable flippable;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        flippable = GetComponent<Flippable>();

        Time.fixedDeltaTime = 1f / Screen.currentResolution.refreshRate;

        flippable.BeginFlip += () =>
        {
            var pos = transform.position;
        };

        flippable.EndFlip += () =>
        {
            grounded = ceilinged = false;
            velocity = Vector2.zero;
        };
    }

    private void FixedUpdate()
    {
        if (flippable.flipping)
        {
            body.velocity = Vector3.zero;
            return;
        }

        float GRAVITY = (2f * JUMP_HEIGHT) / (JUMP_TIME * JUMP_TIME);

        if (jumping_input && grounded)
        {
            jumping = true;
            jump_end = MAX_JUMP_TIME;
            jump_progress = 0;

            velocity.y = (-MAX_JUMP_TIME + Mathf.Sqrt(MAX_JUMP_TIME * MAX_JUMP_TIME - 2.0f * MAX_JUMP_HEIGHT / -GRAVITY)) * GRAVITY;
        }

        if (!jumping_input || ceilinged)
        {
            jump_end = jump_progress;
            jumping = false;
        }

        if (jumping)
        {
            float jump_delta;
            if (jump_progress + Time.fixedDeltaTime >= jump_end)
            {
                jump_delta = jump_end - jump_progress;
                jumping = false;
            }
            else
            {
                jump_delta = Time.fixedDeltaTime;
            }

            jump_progress += jump_delta;
        }
        else if (!grounded)
        {
            velocity.y = Mathf.Max(velocity.y - GRAVITY * Time.fixedDeltaTime, MAX_FALL_SPEED * -1);
        }

        Vector3 target;
        if (movement_input.x > 0)
        {
            target = new Vector3(RUNNING_SPEED_LIMIT, 0);
        }
        else if (movement_input.x < 0)
        {
            target = new Vector3(-RUNNING_SPEED_LIMIT, 0);
        }
        else
        {
            target = new Vector3();
        }

        float speed_change;
        if (Mathf.Abs(velocity.x) > RUNNING_SPEED_LIMIT && Mathf.Sign(velocity.x) == Mathf.Sign(target.x))
        {
            if (grounded)
                speed_change = GROUND_DECELERATION;
            else
                speed_change = AIR_DECELERATION;
        }
        else
        {
            if (grounded)
                speed_change = GROUND_RUNNING_ACCELERATION;
            else if (Mathf.Sign(velocity.x) == -Mathf.Sign(target.x))
                speed_change = AIR_RUNNING_ACCELERATION;
            else
                speed_change = AIR_DECELERATION;
        }

        if (target.x > velocity.x)
        {
            velocity.x = Mathf.Min(velocity.x + speed_change * Time.fixedDeltaTime, target.x);
        }
        else
        {
            velocity.x = Mathf.Max(velocity.x - speed_change * Time.fixedDeltaTime, target.x);
        }

        body.velocity = Quaternion.FromToRotation(Vector3.down, flippable.down) * velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.enabled) return;

        var n = collision.contacts[0].normal;
        n.x = Mathf.Round(n.x);
        n.y = Mathf.Round(n.y);

        if ((n.x == 0) == (flippable.down.x == 0))
        {
            velocity.y = 0;
            if (n == flippable.down)
                ceilinged = true;
            else
                grounded = true;
        }
        else
        {
            velocity.x = 0;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.enabled) return;

        var n = collision.contacts[0].normal;
        n.x = Mathf.Round(n.x);
        n.y = Mathf.Round(n.y);

        if ((n.x == 0) == (flippable.down.x == 0))
        {
            velocity.y = 0;
            if (n == flippable.down)
                ceilinged = true;
            else
                grounded = true;
        }
        else
        {
            velocity.x = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
        ceilinged = false;
    }


    public void SetMovement(Vector2 input)
    {
        movement_input = input;
    }

    public void SetJump(bool jumpPressed)
    {
        jumping_input = jumpPressed;
    }


}
