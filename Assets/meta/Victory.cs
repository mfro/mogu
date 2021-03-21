﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerController>();
        var physics = collision.GetComponent<PhysicsObject>();

        if (player != null && physics?.grounded == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
