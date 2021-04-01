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

    private static Quaternion[] flips = new Quaternion[] {
        rotations[0] * Quaternion.Euler(180, 0, 0),
        rotations[1] * Quaternion.Euler(180, 0, 0),
        rotations[2] * Quaternion.Euler(180, 0, 0),
        rotations[3] * Quaternion.Euler(180, 0, 0),
    };

    private static (Quaternion[], int) GetRotation(Transform t)
    {
        var down = t.localRotation * Vector2.down;
        var left = t.localRotation * Vector2.left;

        for (var i = 0; i < rotations.Length; ++i)
        {
            var match = Vector2.Dot(down, rotations[i] * Vector2.down) > 0.5f
                && Vector2.Dot(left, rotations[i] * Vector2.left) > 0.5f;

            if (match) return (rotations, i);
        }

        for (var i = 0; i < flips.Length; ++i)
        {
            var match = Vector2.Dot(down, flips[i] * Vector2.down) > 0.5f
                && Vector2.Dot(left, flips[i] * Vector2.left) > 0.5f;

            if (match) return (flips, i);
        }

        throw new Exception("no rotation");
    }

    [MenuItem("mushroom/rotate clockwise")]
    private static void RotateCW()
    {
        Undo.RecordObjects(Selection.transforms, "Rotate clockwise");

        foreach (var t in Selection.transforms)
        {
            var (list, i) = GetRotation(t);
            t.localRotation = list[(i + 3) % rotations.Length];
        }
    }

    [MenuItem("mushroom/rotate counter-clockwise")]
    private static void RotateCCW()
    {
        Undo.RecordObjects(Selection.transforms, "Rotate counter-clockwise");

        foreach (var t in Selection.transforms)
        {
            var (list, i) = GetRotation(t);
            t.localRotation = list[(i + 1) % rotations.Length];
        }
    }

    [MenuItem("mushroom/flip vertical")]
    private static void FlipVertical()
    {
        Undo.RecordObjects(Selection.transforms, "Flip vertical");

        foreach (var t in Selection.transforms)
        {
            var (list, i) = GetRotation(t);
            var other = list == rotations ? flips : rotations;
            t.localRotation = other[i % rotations.Length];
        }
    }
}
