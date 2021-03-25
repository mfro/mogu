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
    public static float JUMP_HEIGHT = 34;

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

            if (collider.GetComponent<Door>()?.IsOpen == false)
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

    public static IEnumerable<(MyCollider, MyCollider, Rect)> AllOverlaps(IEnumerable<MyCollider> list, CollideReason reason)
    {
        foreach (var collider in list)
        {
            foreach (var (other, overlap) in AllOverlaps(collider, reason))
            {
                yield return (collider, other, overlap);
            }
        }
    }

    public static IEnumerable<(MyCollider, float)> BoxCast(Rect box, Vector2 motion)
    {
        var stretch = box;
        if (motion.y > 0) stretch.yMax += motion.y;
        else if (motion.y < 0) stretch.yMin += motion.y;
        else if (motion.x > 0) stretch.xMax += motion.x;
        else if (motion.x < 0) stretch.xMin += motion.x;
        else throw new Exception("unsupported box cast");

        var collisions = AllOverlaps(stretch, CollideReason.Collision);

        if (motion.y > 0) collisions = collisions.OrderBy(c => c.Item2.yMin);
        else if (motion.y < 0) collisions = collisions.OrderByDescending(c => c.Item2.yMax);
        else if (motion.x > 0) collisions = collisions.OrderBy(c => c.Item2.xMin);
        else if (motion.x < 0) collisions = collisions.OrderByDescending(c => c.Item2.xMax);
        else throw new Exception("unsupported box cast");

        return collisions.Where(c =>
        {
            if (motion.x != 0) return c.Item2.height > 1e-6;
            if (motion.y != 0) return c.Item2.width > 1e-6;
            throw new Exception("unsupported box cast");
        })
        .Select(c =>
        {
            float distance;
            if (motion.y > 0) distance = c.Item2.yMin - box.yMax;
            else if (motion.y < 0) distance = box.yMin - c.Item2.yMax;
            else if (motion.x > 0) distance = c.Item2.xMin - box.xMax;
            else if (motion.x < 0) distance = box.xMin - c.Item2.xMax;
            else throw new Exception("unsupported box cast");

            return (c.Item1, distance);
        });
    }

    public static Vector2 Slide(MyCollider collider, Vector2 motion)
    {
        var offset = Vector2.zero;
        var objects = new List<MyCollider> { collider };

        while (motion != Vector2.zero)
        {
            var bounds = collider.bounds;

            var (other, distance) = objects.SelectMany(o => BoxCast(o.bounds, motion))
                .Where(c => !objects.Contains(c.Item1))
                .OrderBy(c => c.Item2)
                .FirstOrDefault();

            if (other == null)
            {
                foreach (var o in objects)
                {
                    o.position += motion;
                    o.UpdatePosition();
                }
                return offset;
            }

            if (motion.x != 0)
            {
                Debug.Log($"{distance} / {motion}");
            }

            var movement = motion.normalized * distance;
            motion -= movement;

            var friction = motion * (other.pushRatio - 1);
            motion += friction;
            offset += friction;

            if (motion.x != 0)
                Debug.Log($"collide: {other.bounds.xMin}");

            foreach (var o in objects)
            {
                if (motion.y != 0 || offset.y != 0) o.velocity.y *= other.pushRatio;
                else o.velocity.x *= other.pushRatio;
                o.position += movement;
                o.UpdatePosition();
                if (motion.x != 0)
                    Debug.Log(o.bounds.xMax);
            }

            objects.Add(other);
        }

        return offset;
    }

    public static bool CanMove(MyCollider collider, Vector2 motion)
    {
        if (collider.pushRatio == 0)
            return false;

        var collisions = AllOverlaps(collider.bounds.Shift(motion), CollideReason.Collision)
            .Where(c => c.Item1 != collider);

        return collisions.All(c => CanMove(c.Item1, motion));
    }

    private static Vector2 MoveScaled(MyCollider collider, Vector2 motion)
    {
        collider.remainder += motion;

        var offset = Vector2.zero;
        var objects = new List<MyCollider> { collider };

        while (true)
        {
            var totalRemainder = objects.Sum(c => c.remainder.x);
            if (Mathf.Abs(totalRemainder) < UNIT) break;

            foreach (var o in objects)
                o.remainder.x = 0;
            collider.remainder.x = totalRemainder;

            var sign = Mathf.Sign(collider.remainder.x);
            collider.remainder.x -= sign;

            var (other, overlap) = objects.SelectMany(o => AllOverlaps(o.bounds.ShiftX(sign), CollideReason.Collision))
                .Where(c => !objects.Contains(c.Item1))
                .FirstOrDefault();

            if (other == null)
            {
                foreach (var o in objects)
                {
                    o.position += new Vector2(sign, 0);
                    o.UpdatePosition();
                }
            }
            else if (collider.grounded && other.pushRatio > 0)
            {
                other.remainder.x += sign * other.pushRatio;
                objects.Add(other);
            }
        }

        objects = new List<MyCollider> { collider };
        while (true)
        {
            var totalRemainder = objects.Sum(c => c.remainder.y);
            if (Mathf.Abs(totalRemainder) < UNIT) break;

            foreach (var o in objects)
                o.remainder.y = 0;
            collider.remainder.y = totalRemainder;

            var sign = Mathf.Sign(collider.remainder.y);
            collider.remainder.y -= sign;

            var (other, overlap) = objects.SelectMany(o => AllOverlaps(o.bounds.ShiftY(sign), CollideReason.Collision))
                .Where(c => !objects.Contains(c.Item1))
                .FirstOrDefault();

            if (other == null)
            {
                foreach (var o in objects)
                {
                    o.position += new Vector2(0, sign);
                    o.UpdatePosition();
                }
            }
            else if (other.pushRatio > 0)
            {
                other.remainder.y += sign * other.pushRatio;
                objects.Add(other);
            }
        }

        return offset;
    }

    public static Vector2 Move(MyCollider collider, Vector2 motion)
    {
        var scaled = motion * Time.deltaTime;
        var offset = MoveScaled(collider, scaled);
        // collider.velocity = scaled / Time.deltaTime;
        return offset;
    }
}
