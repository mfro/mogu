using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CollisionMask
{
    None = 0,

    Physical = 1 << 0,
    Flipping = 1 << 1,
    Dynamic = 1 << 2,
    Player = 1 << 3,

    Any = ~0,
}

public static class Physics
{
    public static float MAX_FALL_SPEED = 440;
    public static float RUNNING_SPEED_LIMIT = 150;
    public static float GROUND_RUNNING_ACCELERATION = 2560;
    public static float GROUND_DECELERATION = 7680;
    public static float AIR_RUNNING_ACCELERATION = 1280;
    public static float AIR_DECELERATION = 3840;
    public static float JUMP_TIME = 0.3f;
    public static float JUMP_HEIGHT = 36;

    public static float GRAVITY => (2f * JUMP_HEIGHT) / (JUMP_TIME * JUMP_TIME);
    public static float JUMP_SPEED => GRAVITY * JUMP_TIME;

    public static float SCALE = 32;
    public static float UNIT = 1;

    public static Vector2 ToUnity(Vector2 v) => v / SCALE;
    public static Vector2 FromUnity(Vector2 raw) => Util.Round(raw * SCALE / UNIT) * UNIT;

    public static Rect ToUnity(Rect v) => Util.RectFromCenterSize(ToUnity(v.center), ToUnity(v.size));
    public static Rect FromUnity(Rect raw) => Util.RectFromCenterSize(FromUnity(raw.center), FromUnity(raw.size));

    public static HashSet<MyCollider> allColliders = new HashSet<MyCollider>();

    public static Rect Distance(Rect a, Rect b)
    {
        float xMin = Mathf.Max(a.xMin, b.xMin);
        float yMin = Mathf.Max(a.yMin, b.yMin);

        float xMax = Mathf.Min(a.xMax, b.xMax);
        float yMax = Mathf.Min(a.yMax, b.yMax);

        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }

    public static Rect? Overlap(Rect a, Rect b)
    {
        var rect = Distance(a, b);

        if (rect.width <= 0 || rect.height <= 0)
            return null;

        return rect;
    }

    private static List<MyCollider> paused;
    public static bool IsEnabled
    {
        get => paused == null;
        set
        {
            if (value == IsEnabled) return;

            if (value)
            {
                foreach (var item in paused)
                    item.enabled = true;
                allColliders = new HashSet<MyCollider>(paused);
                paused = null;
            }
            else
            {
                paused = allColliders.ToList();
                foreach (var item in paused)
                    item.enabled = false;
            }
        }
    }

    static Physics()
    {
        SceneController.SceneChanged += () => paused = null;
    }

    public static void Enable(MyCollider collider)
    {
        allColliders.Add(collider);

        foreach (var (other, overlap) in Physics.AllOverlaps(collider))
        {
            collider.touching.Add(other);
            other.touching.Add(collider);
        }
    }

    public static void Disable(MyCollider collider)
    {
        allColliders.Remove(collider);

        foreach (var other in collider.touching)
            other.touching.Remove(collider);
        collider.touching.Clear();
    }

    public static IEnumerable<(MyCollider, Rect)> AllOverlaps(Rect a, CollisionMask mask)
    {
        foreach (var collider in allColliders)
        {
            if ((collider.mask & mask) != CollisionMask.None)
            {
                var overlap = Overlap(collider.bounds, a);
                if (overlap != null)
                {
                    yield return (collider, overlap.Value);
                }
            }
        }
    }

    public static IEnumerable<(MyCollider, Rect)> AllCollisions(Rect a, CollisionMask mask)
    {
        return AllOverlaps(a, mask)
            .Where(c => c.Item1 is MyDynamic || c.Item1 is MyStatic);
    }

    public static IEnumerable<(MyCollider, Rect)> AllOverlaps(MyCollider collider)
    {
        return AllOverlaps(collider.bounds, collider.mask)
            .Where(o => o.Item1 != collider);
    }

    public static IEnumerable<(MyCollider, Rect)> AllCollisions(MyCollider collider)
    {
        return AllOverlaps(collider)
            .Where(c => c.Item1 is MyDynamic || c.Item1 is MyStatic);
    }

    public static bool CanMove(MyDynamic collider, Vector2 motion)
    {
        var bounds = collider.bounds.Shift(motion);

        return AllCollisions(bounds, collider.mask)
            .Where(c => c.Item1 != collider && (c.Item1 is MyStatic || c.Item1 is MyDynamic))
            .Select(c => c.Item1 as MyDynamic)
            .All(d => d != null && CanMove(d, motion));
    }

    public static void Move(MyDynamic collider, Vector2 motion)
    {
        collider.remainder += motion * Time.fixedDeltaTime;

        MoveDimension(collider, 0); // X
        MoveDimension(collider, 1); // Y

        var separated = collider.touching.Select(o => (o, Physics.Distance(collider.bounds, o.bounds)))
            .Where(c => c.Item2.width < 0 || c.Item2.height < 0)
            .ToList();

        foreach (var (other, d) in separated)
        {
            collider.touching.Remove(other);
            other.touching.Remove(collider);
        }
    }

    private static void MoveDimension(MyDynamic collider, int dim)
    {
        var objects = new List<MyDynamic> { collider };

        while (true)
        {
            var totalRemainder = objects.Sum(c => c.remainder[dim]);
            if (Mathf.Abs(totalRemainder) < UNIT) break;

            foreach (var o in objects)
                o.remainder[dim] = 0;
            collider.remainder[dim] = totalRemainder;

            var sign = Mathf.Sign(collider.remainder[dim]) * UNIT;
            collider.remainder[dim] -= sign;

            var shift = new Vector2 { [dim] = sign };
            var collision = false;

            foreach (var obj in objects.ToList())
            {
                var overlaps = AllOverlaps(obj.bounds.Shift(shift), obj.mask)
                    .Where(c => !objects.Contains(c.Item1));

                foreach (var (other, overlap) in overlaps)
                {
                    if (other is MyDynamic || other is MyStatic)
                        collision = true;

                    obj.touching.Add(other);
                    other.touching.Add(obj);

                    if (other is MyDynamic pushee && collider.grounded && pushee.pushRatio > 0)
                    {
                        pushee.remainder[dim] += sign * pushee.pushRatio;
                        collider.remainder[dim] *= pushee.pushRatio;
                        objects.Add(pushee);
                    }
                }
            }

            if (!collision)
            {
                foreach (var o in objects)
                {
                    o.position[dim] += sign;
                    o.UpdatePosition();
                }
            }
        }
    }
}
