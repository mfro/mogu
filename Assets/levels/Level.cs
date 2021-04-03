using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class Level : MonoBehaviour
{
    public string title;
    public Checkpoint start;

#if UNITY_EDITOR
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
