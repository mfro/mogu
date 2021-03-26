using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CollideReason
{
    CellFlip,
    Collision,
    Victory,
    PressurePlate,
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
    public static Vector2 FromUnity(Vector2 raw) => Round(raw * SCALE);

    public static Rect RectFromCenterSize(Vector2 center, Vector2 size) => new Rect(center - size / 2, size);

    public static Rect ShiftX(this Rect rect, float x) => rect.Shift(new Vector2(x, 0));
    public static Rect ShiftY(this Rect rect, float y) => rect.Shift(new Vector2(0, y));
    public static Rect Shift(this Rect rect, Vector2 offset)
    {
        rect.position += offset;
        return rect;
    }

    public static Vector2 ScaleX(this Vector2 v, float x) => new Vector2(v.x * x, v.y);
    public static Vector2 ScaleY(this Vector2 v, float y) => new Vector2(v.x, v.y * y);

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

    public static bool DoCollide(CollideReason reason, MyCollider collider)
    {
        if (reason == CollideReason.Collision)
        {
            if (collider.GetComponent<PressurePlate>() != null)
                return false;

            if (collider.GetComponent<Victory>() != null)
                return false;

            if (collider.GetComponent<Door>()?.IsOpen == true)
                return false;
        }

        if (reason == CollideReason.PressurePlate)
        {
            if (collider.pushRatio == 0)
                return false;
        }

        return true;
    }

    public static IEnumerable<(MyCollider, Rect)> AllOverlaps(Rect a, CollideReason reason)
    {
        foreach (var collider in allColliders)
        {
            var overlap = Overlap(collider.bounds, a);
            if (overlap != null && DoCollide(reason, collider))
            {
                yield return (collider, overlap.Value);
            }
        }
    }

    public static IEnumerable<(MyCollider, Rect)> AllOverlaps(MyCollider collider, CollideReason reason)
    {
        return AllOverlaps(collider.bounds, reason)
            .Where(o => o.Item1 != collider);
    }

    public static bool CanMove(MyCollider collider, Vector2 motion)
    {
        if (collider.pushRatio == 0)
            return false;

        var collisions = AllOverlaps(collider.bounds.Shift(motion), CollideReason.Collision)
            .Where(c => c.Item1 != collider);

        return collisions.All(c => CanMove(c.Item1, motion));
    }

    private static void MoveScaled(MyCollider collider, int dim)
    {
        var objects = new List<MyCollider> { collider };

        while (true)
        {
            var totalRemainder = objects.Sum(c => c.remainder[dim]);
            if (Mathf.Abs(totalRemainder) < UNIT) break;

            foreach (var o in objects)
                o.remainder[dim] = 0;
            collider.remainder[dim] = totalRemainder;

            var sign = Mathf.Sign(collider.remainder[dim]);
            collider.remainder[dim] -= sign;
            var shift = new Vector2 { [dim] = sign };

            var (other, overlap) = objects.SelectMany(o => AllOverlaps(o.bounds.Shift(shift), CollideReason.Collision))
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
            else if (collider.grounded && other.pushRatio > 0)
            {
                other.remainder[dim] += sign * other.pushRatio;
                collider.remainder[dim] *= other.pushRatio;
                objects.Add(other);
            }
        }

    }

    private static void MoveScaled(MyCollider collider, Vector2 motion)
    {
        collider.remainder += motion;

        MoveScaled(collider, 0);
        MoveScaled(collider, 1);
    }

    public static void Move(MyCollider collider, Vector2 motion)
    {
        var scaled = motion * Time.fixedDeltaTime;
        MoveScaled(collider, scaled);
    }
}
