using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerOnContact : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().KillPlayer();
        }
    }
}
