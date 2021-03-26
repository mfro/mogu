using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tools
{
    private static Quaternion[] rotations = new Quaternion[] {
        Quaternion.identity,
        Quaternion.Euler(0, 0, 90),
        Quaternion.Euler(0, 0, 180),
        Quaternion.Euler(0, 0, 270),
    };

    private static int GetRotation(Transform t)
    {
        var down = t.localRotation * Vector2.down;

        for (var i = 0; i < rotations.Length; ++i)
        {
            var test = rotations[i] * Vector2.down;
            var dot = Vector2.Dot(test, down);
            if (dot > 0.5f) return i;
        }

        throw new Exception("no rotation");
    }

    [MenuItem("mushroom/rotate clockwise")]
    private static void RotateCW()
    {
        foreach (var t in Selection.transforms)
        {
            var i = GetRotation(t);
            t.localRotation = rotations[(i + 3) % rotations.Length];
        }
    }

    [MenuItem("mushroom/rotate counter-clockwise")]
    private static void RotateCCW()
    {
        foreach (var t in Selection.transforms)
        {
            var i = GetRotation(t);
            t.localRotation = rotations[(i + 1) % rotations.Length];
        }
    }
}
