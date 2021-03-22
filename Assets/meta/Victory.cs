using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Victory : MonoBehaviour
{
    public Action next;

    private void OnTriggerStay2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerController>();
        var physics = collision.GetComponent<PhysicsObject>();

        if (player != null && physics?.grounded == true)
        {
            gameObject.SetActive(false);
            next();
        }
    }
}
