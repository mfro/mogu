using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public enum FlipKind
{
    CW,
    CCW,
    Vertical,
    Horizontal,
}

public class CellFlip : MonoBehaviour
{
    public static bool isFlipping = false;
    public static Action cancelFlip;

    [SerializeField]
    public FlipError FlipError;

    [SerializeField]
    AudioClip FlipSound;

    [SerializeField]
    public FlipKind flip1;

    [SerializeField]
    public FlipKind flip2;

    [SerializeField]
    float flip_time = 1;

    private AudioSource audioSource;
    private MyCollider physics;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        physics = GetComponent<MyCollider>();
        physics.mask |= CollisionMask.Flipping;
    }

    public void ShowFlipError(Rect bounds)
    {
        var error = Instantiate(FlipError);
        error.transform.position = (Vector3)Physics.ToUnity(bounds.center) + new Vector3(0, 0, -5);
        error.transform.localScale = Physics.ToUnity(bounds.size);
        error.startTime = Time.time;
    }

    public async void DoFlip(Vector2 down, int input)
    {
        FlipKind flip;
        if (input == 1)
            flip = flip1;
        else
            flip = flip2;


        Quaternion delta;
        if (flip == FlipKind.CW)
        {
            delta = Quaternion.AngleAxis(90, Vector3.back);
        }
        else if (flip == FlipKind.CCW)
        {
            delta = Quaternion.AngleAxis(-90, Vector3.back);
        }
        else if (flip == FlipKind.Horizontal)
        {
            delta = Quaternion.AngleAxis(-180, down);
        }
        else if (flip == FlipKind.Vertical)
        {
            var axis = Quaternion.AngleAxis(90, Vector3.back) * down;
            delta = Quaternion.AngleAxis(-180, axis);
        }
        else return;

        if (isFlipping) return;

        var allObjects = Resources.FindObjectsOfTypeAll<Flippable>();

        var overlaps = Physics.AllOverlaps(physics)
            .Select(o => (o.Item1, o.Item2, o.Item1.GetComponent<Flippable>()))
            .Where(o => o.Item3 != null)
            .ToList();

        var partials = overlaps
            .Where(o => o.Item1.bounds != o.Item2)
            .ToList();

        if (partials.Any())
        {
            foreach (var partial in partials)
                ShowFlipError(partial.Item2);
            return;
        }

        var objects = overlaps.Select(o => o.Item3).ToList();

        isFlipping = true;
        audioSource.PlayOneShot(FlipSound);
        var levelController = FindObjectOfType<LevelController>();
        levelController?.SaveUndoState();

        var parents = objects.Select(o => o.transform.parent).ToList();

        foreach (var o in objects)
        {
            o.transform.parent = transform;
            o.DoBeginFlip();
        }

        var cancelled = false;
        cancelFlip = () =>
        {
            cancelled = true;
            foreach (var (o, parent) in objects.Zip(parents, (l, r) => (l, r)))
            {
                o.transform.parent = parent;
                o.DoEndFlip(delta);
            }
            cancelFlip = null;
        };

        var q0 = transform.rotation;
        var q1 = delta * q0;

        float t0 = Time.time;
        while (Time.time - t0 < flip_time)
        {
            transform.rotation = Quaternion.Lerp(q0, q1, (Time.time - t0) / flip_time);
            await Task.Yield();

            if (cancelled)
            {
                isFlipping = false;
                return;
            }
        }

        transform.rotation = q1;

        cancelFlip();

        foreach (var o in objects)
        {
            var dyn = o.GetComponentInChildren<MyDynamic>();
            if (dyn != null && Physics.AllCollisions(dyn).Any())
            {
                levelController.DoUndo();
                break;
            }
        }

        transform.rotation = q0;

        isFlipping = false;
    }
}
