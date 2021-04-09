using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Vector2 Round(Vector2 v)
    {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        return v;
    }

    public static Vector3 Round(Vector3 v)
    {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        v.z = Mathf.Round(v.z);
        return v;
    }

    public static Rect RectFromCenterSize(Vector2 center, Vector2 size) => new Rect(center - size / 2, size);

    public static Rect ShiftX(this Rect rect, float x) => rect.Shift(new Vector2(x, 0));
    public static Rect ShiftY(this Rect rect, float y) => rect.Shift(new Vector2(0, y));
    public static Rect Shift(this Rect rect, Vector2 offset)
    {
        rect.position += offset;
        return rect;
    }

    public static IEnumerable<GameObject> GetChildren(this GameObject o) => GetChildren<GameObject>(o);
    public static IEnumerable<Child> GetChildren<Child>(this GameObject o)
    {
        for (var i = 0; i < o.transform.childCount; ++i)
        {
            var c = o.transform.GetChild(i).GetComponent<Child>();
            if (c != null) yield return c;
        }
    }

    public static void GetComponent<T>(MonoBehaviour self, out T field)
    {
        field = self.GetComponent<T>();
    }

    public static void GetComponentInChildren<T>(MonoBehaviour self, out T field)
    {
        field = self.GetComponentInChildren<T>();
    }
}
