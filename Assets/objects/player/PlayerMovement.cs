using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool input_jumping;
    public float input_running;

    public bool jumping;

    private MyDynamic dyn;
    private Flippable flip;
    private PlayerController controller;

    void Awake()
    {
        Util.GetComponent(this, out dyn);
        Util.GetComponent(this, out flip);
        Util.GetComponent(this, out controller);
    }

    void FixedUpdate()
    {
        if (!dyn.enabled) return;

        if (dyn.grounded) jumping = false;

        if (input_jumping && dyn.grounded)
        {

            controller.PlayJumpSound();

            input_jumping = false;
            if (flip.down.x != 0)
            {
                dyn.velocity.x = 0;
                dyn.remainder.x = 0;
            }
            else
            {
                dyn.velocity.y = 0;
                dyn.remainder.y = 0;
            }

            dyn.velocity += Physics.JUMP_SPEED * -flip.down;
            jumping = true;
        }

        var target = 0f;
        if (input_running > 0)
        {
            target = Physics.RUNNING_SPEED_LIMIT;
        }
        else if (input_running < 0)
        {
            target = -Physics.RUNNING_SPEED_LIMIT;
        }

        float horizontal;
        if (flip.down.y != 0)
            horizontal = dyn.velocity.x;
        else
            horizontal = dyn.velocity.y;

        float speed_change;
        if (Mathf.Abs(horizontal) > Physics.RUNNING_SPEED_LIMIT && Mathf.Sign(horizontal) == Mathf.Sign(target))
        {
            if (dyn.grounded)
                speed_change = Physics.GROUND_DECELERATION;
            else
                speed_change = Physics.AIR_DECELERATION;
        }
        else
        {
            if (dyn.grounded)
                speed_change = Physics.GROUND_RUNNING_ACCELERATION;
            else if (Mathf.Sign(horizontal) == -Mathf.Sign(target))
                speed_change = Physics.AIR_RUNNING_ACCELERATION;
            else
                speed_change = Physics.AIR_DECELERATION;
        }

        if (flip.down.x == 0)
        {
            if (target > dyn.velocity.x)
            {
                dyn.velocity.x = Mathf.Min(dyn.velocity.x + speed_change * Time.fixedDeltaTime, target);
            }
            else
            {
                dyn.velocity.x = Mathf.Max(dyn.velocity.x - speed_change * Time.fixedDeltaTime, target);
            }
        }
        else
        {
            if (target > dyn.velocity.x)
            {
                dyn.velocity.y = Mathf.Min(dyn.velocity.y + speed_change * Time.fixedDeltaTime, target);
            }
            else
            {
                dyn.velocity.y = Mathf.Max(dyn.velocity.y - speed_change * Time.fixedDeltaTime, target);
            }
        }
    }
}
