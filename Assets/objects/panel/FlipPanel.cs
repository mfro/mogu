using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum FlipKind
{
    CW,
    CCW,
    Vertical,
    Horizontal,
}

public class FlipPanel : MonoBehaviour
{
    public static FlipPanel isFlipping;
    public static Action cancelFlip;
    public static float FlipTime = 0.4f;

    [SerializeField]
    public FlipError FlipError;

    [SerializeField]
    AudioClip FlipSound;

    [SerializeField]
    public FlipKind flip1;

    [SerializeField]
    public FlipKind flip2;

    private AudioSource audioSource;
    [NonSerialized] public MyCollider physics;

    void Awake()
    {
        Util.GetComponent(this, out audioSource);
        Util.GetComponent(this, out physics);
        physics.mask |= CollisionMask.Flipping;
    }

    public void ShowFlipError(Rect bounds)
    {
        var error = Instantiate(FlipError);
        error.transform.position = (Vector3)Physics.ToUnity(bounds.center) + new Vector3(0, 0, -5);
        error.transform.localScale = Physics.ToUnity(bounds.size);
        CameraShake.instance.DoShake(CameraShake.FlipError);
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

        if (isFlipping != null) return;

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

        isFlipping = this;
        audioSource.PlayOneShot(FlipSound);
        var levelController = FindObjectOfType<LevelController>();
        levelController?.SaveUndoState();

        var parents = objects.Select(o => o.transform.parent).ToList();

        foreach (var o in objects)
        {
            o.transform.SetParent(transform, true);
            o.DoBeginFlip();
        }

        var cancelled = false;
        cancelFlip = () =>
        {
            cancelled = true;
            foreach (var (o, parent) in objects.Zip(parents, (l, r) => (l, r)))
            {
                o.transform.SetParent(parent, true);
                o.DoEndFlip(delta);
            }
            cancelFlip = null;
        };

        var originalPos = transform.position;
        transform.position = originalPos + new Vector3(0, 0, -5);

        var q0 = transform.rotation;
        var q1 = delta * q0;

        await Animations.Animate(FlipTime, true, Animations.EaseInOutSine, progress =>
        {
            transform.rotation = Quaternion.Lerp(q0, q1, progress);
        });

        if (cancelled) return;

        transform.position = originalPos;
        cancelFlip();

        foreach (var o in objects)
        {
            var collider = o.GetComponentInChildren<MyCollider>();
            if (collider is MyStatic || collider is MyDynamic)
            {
                var (other, overlap) = Physics.AllCollisions(collider).FirstOrDefault();

                if (other != null)
                {
                    ShowFlipError(overlap);
                    levelController.DoUndo();
                    break;
                }
            }
        }

        isFlipping = null;
    }
}
