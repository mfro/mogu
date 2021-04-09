using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{

    public Vector2 velocity;
    // Start is called before the first frame update

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Physics.IsEnabled) transform.position += new Vector3(velocity.x * Time.fixedDeltaTime, velocity.y * Time.fixedDeltaTime, 0);

        GetComponent<Animator>().enabled = Physics.IsEnabled;


        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (velocity.x >= 0) sr.flipX = true;
        else sr.flipX = false;
    }
}
