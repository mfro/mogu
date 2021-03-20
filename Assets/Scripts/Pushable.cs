using System.Collections;
using System.Collections.Generic;
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

    private Vector2 currentPushVelocity = Vector2.zero;
    private Vector2 currentGravityVelocity = Vector2.zero;
    private Vector2 downVector = Vector2.down;

    private bool startDecreasing;
    private float currTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        flip = GetComponent<Flippable>();
        col = GetComponent<Collider2D>();

        flip.BeginFlip += () =>
        {
        };

        flip.EndFlip += () =>
        {
            downVector = flip.down;
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, transform.localScale, 0, downVector, 0.001f);

        if (hits.Length == 1)
        {
            currentGravityVelocity = downVector * gravityVelocity;
        } else
        {
            currentGravityVelocity = Vector2.zero;
        }

        if (startDecreasing)
        {
            currentPushVelocity = Vector2.Lerp(currentPushVelocity, Vector2.zero, currTime / accelTime);

            currTime += Time.fixedDeltaTime;

            if (currTime >= accelTime)
            {
                startDecreasing = false;
                currentPushVelocity = Vector2.zero;
            }
        }

        rb.velocity = currentGravityVelocity + currentPushVelocity;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        int numContacts = collision.GetContacts(contacts);

        if (numContacts == 0) return;

        if (collision.gameObject.CompareTag("Player"))
        {

            if (collision.gameObject.GetComponent<PlayerMovement>().grounded == false)
            {
                currentPushVelocity = Vector2.zero;
                return;
            }

            foreach (var contact in contacts)
            {
                if (flip.down.x == 0)
                {
                    if (Mathf.Abs(contact.normal.x) >= 0.001f)
                    {
                        startDecreasing = false;
                        currentPushVelocity = (pushVelocity * contact.normal);
                        return;
                    }
                }
                else
                {
                    if (Mathf.Abs(contact.normal.y) >= 0.001f)
                    {
                        startDecreasing = false;
                        Debug.DrawRay(contact.point, contact.normal * pushVelocity, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
                        currentPushVelocity = (pushVelocity * contact.normal);
                        return;
                    }
                }
            }
        }

        else
        {
            
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            startDecreasing = true;
            currTime = 0f;
        }

    }
}
