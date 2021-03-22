using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject start;

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + Vector3.up, 1);
    }

    [DrawGizmo(GizmoType.Active | GizmoType.NonSelected, typeof(Level))]
    public static void MyGizmo(Level level, GizmoType type)
    {
        // Gizmos.DrawWireCube(transform.position, transform.lossyScale * 12);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(level.transform.position, level.transform.lossyScale * 12);
    }
    #endif
}
