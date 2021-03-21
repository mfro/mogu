using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    private bool isActive;
    public bool IsActive
    {
        get { return isActive; }
        protected set
        {
            if (value != isActive)
            {
                isActive = value;
                StateChanged?.Invoke(value);
            }
        }
    }

    public event Action<bool> StateChanged;
}
