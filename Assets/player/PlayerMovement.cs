using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector2 movement_input;
    public bool jumping_input;

    private Flippable flip;
    private MyCollider physics;
    private PlayerController controller;

    private void Start()
    {
        flip = GetComponent<Flippable>();
        physics = GetComponent<MyCollider>();
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (flip.flipping)
        {
            return;
        }

        if (jumping_input && physics.grounded)
        {
            jumping_input = false;
            physics.velocity += Physics.JUMP_SPEED * -flip.down;
            physics.grounded = false;
        }

        var target = 0f;
        if (movement_input.x > 0)
        {
            target = Physics.RUNNING_SPEED_LIMIT;
        }
        else if (movement_input.x < 0)
        {
            target = -Physics.RUNNING_SPEED_LIMIT;
        }
        else if (flip.down.x != 0)
        {
            if (movement_input.y > 0)
            {
                target = Physics.RUNNING_SPEED_LIMIT;
            }
            else if (movement_input.y < 0)
            {
                target = -Physics.RUNNING_SPEED_LIMIT;
            }
        }

        float speed_change;
        if (Mathf.Abs(physics.velocity.x) > Physics.RUNNING_SPEED_LIMIT && Mathf.Sign(physics.velocity.x) == Mathf.Sign(target))
        {
            if (physics.grounded)
                speed_change = Physics.GROUND_DECELERATION;
            else
                speed_change = Physics.AIR_DECELERATION;
        }
        else
        {
            if (physics.grounded)
                speed_change = Physics.GROUND_RUNNING_ACCELERATION;
            else if (Mathf.Sign(physics.velocity.x) == -Mathf.Sign(target))
                speed_change = Physics.AIR_RUNNING_ACCELERATION;
            else
                speed_change = Physics.AIR_DECELERATION;
        }

        if (flip.down.x == 0)
        {
            if (target > physics.velocity.x)
            {
                physics.velocity.x = Mathf.Min(physics.velocity.x + speed_change * Time.deltaTime, target);
            }
            else
            {
                physics.velocity.x = Mathf.Max(physics.velocity.x - speed_change * Time.deltaTime, target);
            }
        }
        else
        {
            if (target > physics.velocity.x)
            {
                physics.velocity.y = Mathf.Min(physics.velocity.y + speed_change * Time.deltaTime, target);
            }
            else
            {
                physics.velocity.y = Mathf.Max(physics.velocity.y - speed_change * Time.deltaTime, target);
            }
        }
    }
}
