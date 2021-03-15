using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Flippable : MonoBehaviour
{
    public event Action BeginFlip;
    public event Action EndFlip;
    public bool flipping = false;
    public Vector2 down = Vector2.down;

    private Vector3 scaleSave;
    private bool snapPosition;

    // Start is called before the first frame update
    void Start()
    {
        scaleSave = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DoBeginFlip()
    {
        BeginFlip?.Invoke();
        flipping = true;

        snapPosition = transform.localPosition.x % 0.5f == 0
            && transform.localPosition.x % 0.5f == 0
            && transform.localPosition.x % 0.5f == 0;
    }

    public void DoEndFlip(Quaternion delta)
    {
        down = delta * down;
        down.x = Mathf.Round(down.x);
        down.y = Mathf.Round(down.y);
        flipping = false;
        EndFlip?.Invoke();

        transform.localScale = scaleSave;
        var angles = transform.localRotation.eulerAngles;
        angles.x = Mathf.Round(angles.x);
        angles.y = Mathf.Round(angles.y);
        angles.z = Mathf.Round(angles.z);
        transform.localRotation = Quaternion.Euler(angles);

        if (snapPosition)
        {
            var pos = transform.localPosition * 2;
            pos.x = Mathf.Round(pos.x);
            pos.y = Mathf.Round(pos.y);
            pos.z = Mathf.Round(pos.z);
            transform.localPosition = pos / 2;
        }
    }
}
