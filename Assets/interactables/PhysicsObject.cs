using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsObject : MonoBehaviour
{
    public static float MAX_FALL_SPEED = 20f;
    public static float RUNNING_SPEED_LIMIT = 5f;
    public static float GROUND_RUNNING_ACCELERATION = 80f;
    public static float GROUND_DECELERATION = 240f;
    public static float AIR_RUNNING_ACCELERATION = 40f;
    public static float AIR_DECELERATION = 120f;
    public static float JUMP_TIME = 0.3f;
    public static float JUMP_HEIGHT = 1.0f;

    public static float MAX_JUMP_TIME = 0f;
    public static float MAX_JUMP_HEIGHT = 1.0f;

    public static float GRAVITY => (2f * JUMP_HEIGHT) / (JUMP_TIME * JUMP_TIME);

    public bool grounded;
    public bool ceilinged;
    public Vector2 velocity;

    private Rigidbody2D body;
    private Flippable flippable;

    private Vector2 down => flippable?.down ?? Vector2.down;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        flippable = GetComponent<Flippable>();

        if (flippable != null)
        {
            flippable.EndFlip += () =>
            {
                grounded = ceilinged = false;
                velocity = Vector2.zero;
            };
        }
    }

    private void FixedUpdate()
    {
        if (flippable?.flipping == true)
        {
            body.velocity = Vector3.zero;
            return;
        }

        if (!grounded)
        {
            velocity.y = Mathf.Max(velocity.y - GRAVITY * Time.fixedDeltaTime, MAX_FALL_SPEED * -1);
        }

        body.velocity = Quaternion.FromToRotation(Vector3.down, down) * velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.enabled) return;

        var n = collision.contacts[0].normal;
        n.x = Mathf.Round(n.x);
        n.y = Mathf.Round(n.y);

        if (Vector2.Dot(n, Quaternion.FromToRotation(Vector3.down, down) * velocity) > 0)
            return;

        if (Vector2.Dot(n, down) == 0)
        {
            velocity.x = 0;
        }
        else
        {
            velocity.y = 0;
            if (n == down)
                ceilinged = true;
            else
                grounded = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.enabled) return;

        var n = collision.contacts[0].normal;
        n.x = Mathf.Round(n.x);
        n.y = Mathf.Round(n.y);

        if (Vector2.Dot(n, Quaternion.FromToRotation(Vector3.down, down) * velocity) > 0)
            return;

        if (Vector2.Dot(n, down) == 0)
        {
            velocity.x = 0;
        }
        else
        {
            velocity.y = 0;
            if (n == down)
                ceilinged = true;
            else
                grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
        ceilinged = false;
    }
}
