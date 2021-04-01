using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static void GetComponent<T>(MonoBehaviour self, out T field)
    {
        field = self.GetComponent<T>();
    }

    public static void GetComponentInChildren<T>(MonoBehaviour self, out T field)
    {
        field = self.GetComponentInChildren<T>();
    }
}
