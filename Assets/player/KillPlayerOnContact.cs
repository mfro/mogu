﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KillPlayerOnContact : MonoBehaviour
{
    private MyCollider physics;

    void Start()
    {
        physics = GetComponent<MyCollider>();
    }

    void FixedUpdate()
    {
        var overlapping = Physics.AllOverlaps(physics)
            .Select(o => o.Item1.GetComponent<PlayerHealth>())
            .Where(o => o != null)
            .ToList();

        foreach (var item in overlapping)
        {
            item.KillPlayer();
        }
    }
}
