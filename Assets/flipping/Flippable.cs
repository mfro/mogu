using System;
using UnityEngine;

public class Flippable : MonoBehaviour
{
    public event Action BeginFlip;
    public event Action<Quaternion> EndFlip;
    public bool flipping = false;
    public Vector2 down = Vector2.down;

    private Vector3 scaleSave;
    private bool snapPosition;

    private new Collider2D collider;

    // Start is called before the first frame update
    void Awake()
    {
        scaleSave = transform.localScale;

        collider = GetComponent<Collider2D>();
    }


#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (name.Contains("platform")) return;

        var color = Color.black;
        Gizmos.color = color;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3) down);
    }
#endif

    public void DoBeginFlip()
    {
        flipping = true;

        snapPosition = transform.localPosition.x % 0.5f == 0
            && transform.localPosition.x % 0.5f == 0
            && transform.localPosition.x % 0.5f == 0;

        if (collider != null) collider.enabled = false;

        BeginFlip?.Invoke();
    }

    public void DoEndFlip(Quaternion delta)
    {
        down = Physics.Round(delta * down);
        flipping = false;

        if (collider != null) collider.enabled = true;

        transform.localScale = scaleSave;
        transform.localRotation = Quaternion.Euler(Physics.Round(transform.localRotation.eulerAngles));

        if (snapPosition)
        {
            transform.localPosition = Physics.Round(transform.localPosition * 2) / 2;
        }
        else
        {
            var pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }

        EndFlip?.Invoke(delta);
    }
}
