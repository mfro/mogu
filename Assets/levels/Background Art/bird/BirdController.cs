using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public Vector2 velocity;

    private float remaining;

    void Awake()
    {
        remaining = 120;
    }

    void FixedUpdate()
    {
        if (remaining <= 0)
        {
            Destroy(this);
        }
        else if (Physics.IsEnabled)
        {
            transform.position += new Vector3(velocity.x * Time.fixedDeltaTime, velocity.y * Time.fixedDeltaTime, 0);
            remaining -= Time.fixedDeltaTime;
        }

        GetComponent<Animator>().enabled = Physics.IsEnabled;
    }
}
