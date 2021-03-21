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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        flip = GetComponent<Flippable>();
        col = GetComponent<Collider2D>();
        physics = GetComponent<PhysicsObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (flip.flipping)
            return;

        if (startDecreasing)
        {
            currentPushVelocity = Mathf.Lerp(currentPushVelocity, 0, currTime / accelTime);

            currTime += Time.fixedDeltaTime;

            if (currTime >= accelTime)
            {
                startDecreasing = false;
                currentPushVelocity = 0;
            }
        }

        physics.velocity.x = currentPushVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        int numContacts = collision.GetContacts(contacts);

        if (numContacts == 0) return;

        if (collision.gameObject.CompareTag("Player"))
        {

            if (collision.gameObject.GetComponent<PhysicsObject>().grounded == false)
            {
                currentPushVelocity = 0;
                return;
            }

            foreach (var contact in contacts)
            {
                var push = Quaternion.FromToRotation(flip.down, Vector3.down) * contact.normal;
                if (Mathf.Round(push.x) != 0)
                {
                    startDecreasing = false;
                    currentPushVelocity = Mathf.Sign(push.x);
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            startDecreasing = true;
            currTime = 0f;
        }
    }
}
