using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public delegate float TimingFunction(float t);

public enum AnimationStep
{
    Stop,
    Skip,
    Continue,
}

public static partial class Animations
{
    public static float Linear(float t) => t;
    public static float EaseInOutSine(float t) => (-Mathf.Cos(Mathf.PI * t) + 1) / 2;
    public static float EaseInOutCubic(float t) => t < 0.5 ? (Mathf.Pow(2 * t, 3) / 2) : (Mathf.Pow(2 * t - 2, 3) / 2 + 1);
    public static float EaseInOutQuadratic(float t) => t < 0.5 ? (2 * Mathf.Pow(t, 2)) : (1 - Mathf.Pow(-2 * t + 2, 2) / 2);

    public class Animation
    {
        public bool isComplete => progress == 1;
        public float progress;

        private float elapsed;
        private float duration;
        private TimingFunction timing;

        public Animation(float duration, TimingFunction timing)
        {
            this.duration = duration;
            this.timing = timing;
        }

        private void StepAsync(float step)
        {
            elapsed += step;
            if (elapsed >= duration) progress = 1;
            else progress = timing(elapsed / duration);
        }

        public async Task NextFrame() => StepAsync(await Util.NextFrame());
        public async Task NextFixedUpdate() => StepAsync(await Util.NextFixedUpdate());
    }

    public static Animation Animate(float duration, TimingFunction timing)
    {
        return new Animation(duration, timing);
    }

    public static async Task Animate(float duration, TimingFunction timing, Func<float, AnimationStep> update)
    {
        var elapsed = 0f;
        await Util.EveryFrame(deltaT =>
        {
            var time = Mathf.Clamp(elapsed + deltaT, 0, duration);
            var step = update(timing(time));

            switch (step)
            {
                case AnimationStep.Stop: return false;
                case AnimationStep.Skip: return true;
                case AnimationStep.Continue:
                    if (time >= duration) return false;
                    elapsed = time;
                    return true;
                default: throw new ArgumentOutOfRangeException();
            }
        });

        update(1);
    }
}
