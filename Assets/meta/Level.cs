using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject start;

#if UNITY_EDITOR
    public void Start()
    {
        if (Selection.activeTransform?.IsChildOf(transform) == true)
        {
            var controller = FindObjectOfType<LevelController>();

            controller.SkipToLevel(this);
        }
    }

    public void OnDrawGizmos()
    {
        if (Selection.activeTransform?.IsChildOf(transform) == true)
        {
            var color = Color.yellow;
            color.a = 0.2f;
            Gizmos.color = color;
            Gizmos.DrawWireCube(transform.position, transform.lossyScale * 12);
        }
        else
        {
            var color = Color.white;
            color.a = 0.1f;
            Gizmos.color = color;
            Gizmos.DrawWireCube(transform.position, transform.lossyScale * 12);
        }
    }
#endif
}
