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

    Any = ~0,
}

public static class Physics
{
    public static float MAX_FALL_SPEED = 640;
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

    public static Vector2 ToUnity(Vector2 v) => v / SCALE;
    public static Vector2 FromUnity(Vector2 raw) => Round(raw * SCALE / UNIT) * UNIT;

    public static Rect RectFromCenterSize(Vector2 center, Vector2 size) => new Rect(center - size / 2, size);

    public static Rect ShiftX(this Rect rect, float x) => rect.Shift(new Vector2(x, 0));
    public static Rect ShiftY(this Rect rect, float y) => rect.Shift(new Vector2(0, y));
    public static Rect Shift(this Rect rect, Vector2 offset)
    {
        rect.position += offset;
        return rect;
    }

    public static HashSet<MyCollider> allColliders = new HashSet<MyCollider>();

    public static Rect? Overlap(Rect a, Rect b)
    {
        float xMin = Mathf.Max(a.xMin, b.xMin);
        float yMin = Mathf.Max(a.yMin, b.yMin);

        float xMax = Mathf.Min(a.xMax, b.xMax);
        float yMax = Mathf.Min(a.yMax, b.yMax);

        if (xMin >= xMax || yMin >= yMax)
            return null;

        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }

    public static IEnumerable<(MyCollider, Rect)> AllOverlaps(CollisionMask mask, Rect a)
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

    public static IEnumerable<(MyCollider, Rect)> AllOverlaps(CollisionMask reason, MyCollider collider)
    {
        return AllOverlaps(reason, collider.bounds)
            .Where(o => o.Item1 != collider);
    }

    public static bool CanMove(MyDynamic collider, Vector2 motion)
    {
        return AllOverlaps(CollisionMask.Physical, collider.bounds.Shift(motion))
            .Where(c => c.Item1 != collider)
            .Select(c => c.Item1 as MyDynamic)
            .All(d => d != null && CanMove(d, motion));
    }

    private static void MoveScaled(MyDynamic collider, int dim)
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

            var (other, overlap) = objects.SelectMany(o => AllOverlaps(CollisionMask.Physical, o.bounds.Shift(shift)))
                .Where(c => !objects.Contains(c.Item1))
                .FirstOrDefault();

            if (other == null)
            {
                foreach (var o in objects)
                {
                    o.position[dim] += sign;
                    o.UpdatePosition();
                }
            }
            else if (other is MyDynamic pushee && collider.grounded && pushee.pushRatio > 0)
            {
                pushee.remainder[dim] += sign * pushee.pushRatio;
                collider.remainder[dim] *= pushee.pushRatio;
                objects.Add(pushee);
            }
        }
    }

    private static void MoveScaled(MyDynamic collider, Vector2 motion)
    {
        collider.remainder += motion;

        MoveScaled(collider, 0);
        MoveScaled(collider, 1);
    }

    public static void Move(MyDynamic collider, Vector2 motion)
    {
        var scaled = motion * Time.fixedDeltaTime;
        MoveScaled(collider, scaled);
    }
}
