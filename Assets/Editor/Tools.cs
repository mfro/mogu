using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
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
        if (Application.isPlaying) return;

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
        if (Application.isPlaying) return;

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
        if (Application.isPlaying) return;

        Undo.RecordObjects(Selection.transforms, "Flip vertical");

        foreach (var t in Selection.transforms)
        {
            var (list, i) = GetRotation(t);
            var other = list == rotations ? flips : rotations;
            t.localRotation = other[i % rotations.Length];
        }
    }

    [MenuItem("mushroom/set pickable")]
    private static void SetPickable()
    {
        for (var i = 0; i < SceneManager.sceneCount; ++i)
        {
            var scene = SceneManager.GetSceneAt(i);

            foreach (var o in FindInScene<SceneController>(scene))
            {
                SceneVisibilityManager.instance.DisablePicking(o.gameObject, true);
                // SceneVisibilityManager.instance.Hide(o.gameObject, true);
            }

            foreach (var o in FindInScene<AudioManager>(scene))
            {
                SceneVisibilityManager.instance.DisablePicking(o.gameObject, true);
                // SceneVisibilityManager.instance.Hide(o.gameObject, true);
            }

            foreach (var o in FindInScene<Canvas>(scene))
            {
                SceneVisibilityManager.instance.DisablePicking(o.gameObject, true);
                // SceneVisibilityManager.instance.Hide(o.gameObject, true);
            }

            foreach (var o in FindInScene<Level>(scene))
            {
                SceneVisibilityManager.instance.DisablePicking(o.gameObject, false);
                // SceneVisibilityManager.instance.Hide(o.gameObject, true);
            }
        }

        for (var i = 0; i < SceneManager.sceneCount; ++i)
        {
            foreach (var root in SceneManager.GetSceneAt(i).GetRootGameObjects())
                SetPickable(root);
        }
    }

    private static HashSet<string> immutable = new HashSet<string>
    {
        "Assets/levels/prefabs/level sign.prefab",
        "Assets/levels/prefabs/platform.prefab",
        "Assets/levels/prefabs/platform pressure plate.prefab",
        "Assets/levels/prefabs/door.prefab",
        "Assets/levels/prefabs/crate.prefab",
        "Assets/levels/prefabs/crate tall.prefab",
        "Assets/levels/prefabs/panel flip v.prefab",
        "Assets/levels/prefabs/panel rotate.prefab",
    };

    private static void SetPickable(GameObject o)
    {
        var root = PrefabUtility.GetNearestPrefabInstanceRoot(o);
        var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(o);

        if (root == o && immutable.Contains(path))
        {
            SceneVisibilityManager.instance.DisablePicking(o.gameObject, true);
            SceneVisibilityManager.instance.EnablePicking(o.gameObject, false);
        }
        else
        {
            if (root == o)
            {
                Debug.Log(path);
            }

            foreach (var child in o.GetChildren())
            {
                SetPickable(child);
            }
        }
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Pickable)]
    private static void DrawPrefabGizmo(GameObject o, GizmoType type)
    {
        var root = PrefabUtility.GetNearestPrefabInstanceRoot(o);
        var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(o);

        if (root == o && immutable.Contains(path) && !Selection.gameObjects.Contains(o))
        {
            Gizmos.color = new Color(0.25f, 0, 0.25f, 0);
            Gizmos.DrawCube(o.transform.position, o.transform.lossyScale);
        }
    }

    [MenuItem("mushroom/internal flip")]
    private static void InternalFlip()
    {
        var target = Selection.activeTransform;

        for (var i = 0; i < target.childCount; ++i)
        {
            var inner = target.GetChild(i);
            Undo.RecordObject(inner, "Internal flip");

            var pos = inner.localPosition;
            pos.y = -pos.y;
            inner.localPosition = pos;
        }
    }

    [MenuItem("mushroom/internal rotate clockwise")]
    private static void InternalRotateCW() => InternalRotate(Quaternion.AngleAxis(90, Vector3.back));

    [MenuItem("mushroom/internal rotate counter-clockwise")]
    private static void InternalRotateCCW() => InternalRotate(Quaternion.AngleAxis(-90, Vector3.back));

    private static void InternalRotate(Quaternion delta)
    {
        var target = Selection.activeTransform;

        for (var i = 0; i < target.childCount; ++i)
        {
            var inner = target.GetChild(i);
            Undo.RecordObject(inner, "Internal flip");

            var pos = inner.localPosition;
            pos = Util.Round(delta * pos * 2) / 2;
            inner.localPosition = pos;
        }
    }

    private static IEnumerable<T> FindInScene<T>(Scene scene)
    {
        return scene.GetRootGameObjects()
            .SelectMany(o => o.GetComponentsInChildren<T>());
    }
}
