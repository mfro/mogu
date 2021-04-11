using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public delegate float TimingFunction(float t);

public static partial class Animations
{
    public static float Linear(float t) => t;
    public static float EaseInOutSine(float t) => (-Mathf.Cos(Mathf.PI * t) + 1) / 2;
    public static float EaseInOutCubic(float t) => t < 0.5 ? (Mathf.Pow(2 * t, 3) / 2) : (Mathf.Pow(2 * t - 2, 3) / 2 + 1);
    public static float EaseInOutQuadratic(float t) => t < 0.5 ? (2 * Mathf.Pow(t, 2)) : (1 - Mathf.Pow(-2 * t + 2, 2) / 2);

    public static async Task Animate(float duration, bool physicsBound, TimingFunction fn, Action<float> update)
    {
        var elapsed = 0f;
        await Util.EveryFrame(deltaT =>
        {
            if (physicsBound && !Physics.IsEnabled) return true;
            elapsed += deltaT;
            if (elapsed >= duration) return false;

            var t = fn(elapsed / duration);
            update(t);

            return true;
        });

        update(1);
    }
}
