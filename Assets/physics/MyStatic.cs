using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyStatic : MyCollider
{
    void Awake()
    {
        mask |= CollisionMask.Physical;
    }
}
