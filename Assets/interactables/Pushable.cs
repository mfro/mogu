using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Pushable : MonoBehaviour
{
    [SerializeField] float pushVelocity;
    [SerializeField] float gravityVelocity;
    [SerializeField] float accelTime;

    private Rigidbody2D rb;
    private Flippable flip;
    private Collider2D col;
    private PhysicsObject physics;

    private float currentPushVelocity = 0f;

    private bool startDecreasing;
    private float currTime;

    public PhysicsObject pushing;
    public PlayerController playerPushing;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        flip = GetComponent<Flippable>();
        col = GetComponent<Collider2D>();
        physics = GetComponent<PhysicsObject>();

        pushing = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (flip.flipping)
            return;

        // if (pushing != null)
        // {
        //     print("being pushed");

        //     physics.velocity.x = pushing.velocity.x;
        // }
        // else
        // {
        //     physics.velocity.x = 0;

        //     // var velocity = rb.velocity;

        //     // if (flip.down.x == 0)
        //     //     velocity.x = 0;
        //     // else
        //     //     velocity.y = 0;

        //     // rb.velocity = velocity;
        // }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var normals = collision.contacts.Where(c => Vector2.Dot(c.normal, flip.down) == 0);

        // var normal = collision.contacts.Any(n => Vector2.Dot(n.normal, flip.down) == 0);
        // if (!normal) return;

        // var player = collision.gameObject.GetComponent<PlayerController>();
        // var pushing = collision.gameObject.GetComponent<PhysicsObject>();

        // if (pushing != null && pushing.grounded)
        // {
        //     this.pushing = pushing;

        //     if (player != null)
        //     {
        //         playerPushing = player;
        //         player.encumbered = true;
        //     }
        // }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // if (collision.gameObject == pushing?.gameObject)
        // {
        //     if (playerPushing)
        //         playerPushing.encumbered = false;

        //     playerPushing = null;
        //     pushing = null;
        // }
    }
}
