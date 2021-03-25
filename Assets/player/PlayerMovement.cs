using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public bool jumping = false;
    public float jump_end = 0;
    public float jump_progress = 0;

    public Vector2 movement_input;
    public bool jumping_input;

    private Rigidbody2D body;
    private new BoxCollider2D collider;
    private Flippable flippable;
    private PhysicsObject physics;
    private PlayerController controller;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        flippable = GetComponent<Flippable>();
        physics = GetComponent<PhysicsObject>();
        controller = GetComponent<PlayerController>();

        Time.fixedDeltaTime = 1f / Screen.currentResolution.refreshRate;
    }

    private void FixedUpdate()
    {
        if (flippable.flipping)
            return;

        if (jumping_input && physics.grounded)
        {
            jumping = true;
            jump_end = PhysicsObject.MAX_JUMP_TIME;
            jump_progress = 0;

            physics.velocity.y = (-PhysicsObject.MAX_JUMP_TIME + Mathf.Sqrt(PhysicsObject.MAX_JUMP_TIME * PhysicsObject.MAX_JUMP_TIME - 2.0f * PhysicsObject.MAX_JUMP_HEIGHT / -PhysicsObject.GRAVITY)) * PhysicsObject.GRAVITY;
        }

        if (!jumping_input || physics.ceilinged)
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

        var target = 0f;
        if (movement_input.x > 0)
        {
            target = PhysicsObject.RUNNING_SPEED_LIMIT;
        }
        else if (movement_input.x < 0)
        {
            target = -PhysicsObject.RUNNING_SPEED_LIMIT;
        }
        else if (flippable.down.x > 0)
        {
            if (movement_input.y > 0)
            {
                target = PhysicsObject.RUNNING_SPEED_LIMIT;
            }
            else if (movement_input.y < 0)
            {
                target = -PhysicsObject.RUNNING_SPEED_LIMIT;
            }
        }
        else if (flippable.down.x < 0)
        {
            if (movement_input.y > 0)
            {
                target = -PhysicsObject.RUNNING_SPEED_LIMIT;
            }
            else if (movement_input.y < 0)
            {
                target = PhysicsObject.RUNNING_SPEED_LIMIT;
            }
        }

        if (controller.encumbered)
            target *= PhysicsObject.ENCUMBERED_MULTIPLIER;

        float speed_change;
        if (Mathf.Abs(physics.velocity.x) > PhysicsObject.RUNNING_SPEED_LIMIT && Mathf.Sign(physics.velocity.x) == Mathf.Sign(target))
        {
            if (physics.grounded)
                speed_change = PhysicsObject.GROUND_DECELERATION;
            else
                speed_change = PhysicsObject.AIR_DECELERATION;
        }
        else
        {
            if (physics.grounded)
                speed_change = PhysicsObject.GROUND_RUNNING_ACCELERATION;
            else if (Mathf.Sign(physics.velocity.x) == -Mathf.Sign(target))
                speed_change = PhysicsObject.AIR_RUNNING_ACCELERATION;
            else
                speed_change = PhysicsObject.AIR_DECELERATION;
        }

        if (controller.encumbered)
            speed_change *= PhysicsObject.ENCUMBERED_MULTIPLIER;

        if (target > physics.velocity.x)
        {
            physics.velocity.x = Mathf.Min(physics.velocity.x + speed_change * Time.fixedDeltaTime, target);
        }
        else
        {
            physics.velocity.x = Mathf.Max(physics.velocity.x - speed_change * Time.fixedDeltaTime, target);
        }
    }
}
