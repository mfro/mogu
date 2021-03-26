using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    public Action next;

    private MyCollider physics;

    void Start()
    {
        physics = GetComponent<MyCollider>();
    }

    void FixedUpdate()
    {
        if (!physics.enabled) return;

        var (player, _) = Physics.AllOverlaps(physics, CollideReason.Victory)
            .FirstOrDefault(c => c.Item1.GetComponent<PlayerController>() != null);

        if (player != null && player.grounded == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
