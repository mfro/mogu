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

    [SerializeField]
    GameObject hints;

    private GameObject hintCW;
    private GameObject hintCCW;
    private GameObject hintVertical;
    private GameObject hintHorizontal;

    private AudioSource audioSource;
    private MyCollider physics;

    void Awake()
    {
        Util.GetComponent(this, out audioSource);
        Util.GetComponent(this, out physics);
        physics.mask |= CollisionMask.Flipping;
    }

    void Start()
    {
        hints.transform.parent = null;
        hints.transform.localScale = Vector3.one;

        hintCW = hints.transform.GetChild(0).gameObject;
        hintCCW = hints.transform.GetChild(1).gameObject;
        hintVertical = hints.transform.GetChild(2).gameObject;
        hintHorizontal = hints.transform.GetChild(3).gameObject;

        var x = transform.localScale.x / 2;
        var y = transform.localScale.y / 2;

        hintCW.transform.localPosition = new Vector3(x - 1 / 64f, y - 1 / 64f, -5);
        hintCW.SetActive(flip1 == FlipKind.CW || flip2 == FlipKind.CW);

        hintCCW.transform.localPosition = new Vector3(-x + 1 / 64f, y - 1 / 64f, -5);
        hintCCW.SetActive(flip1 == FlipKind.CCW || flip2 == FlipKind.CCW);

        hintVertical.transform.localPosition = new Vector3(1 / 64f, y - 1 / 64f, -5);
        hintVertical.SetActive(flip1 == FlipKind.Vertical || flip2 == FlipKind.Vertical);

        hintHorizontal.transform.localPosition = new Vector3(1 / 64f, y - 1 / 64f, -5);
        hintHorizontal.SetActive(flip1 == FlipKind.Horizontal || flip2 == FlipKind.Horizontal);
    }

    void Update()
    {
        var player = physics.touching
            .Where(o => o.GetComponent<PlayerController>() != null)
            .FirstOrDefault();

        if (player == null)
        {
            hints.SetActive(isFlipping == this);
        }
        else
        {
            var overlap = Physics.Overlap(player.bounds, physics.bounds);

            if (overlap != player.bounds)
            {
                hints.SetActive(isFlipping == this);
            }
            else
            {
                hints.SetActive(true);

                if (flip1 == FlipKind.Vertical || flip2 == FlipKind.Vertical)
                {
                    hintVertical.SetActive(player.flip.down.x == 0);
                    hintHorizontal.SetActive(player.flip.down.y == 0);
                }
                else if (flip1 == FlipKind.Horizontal || flip2 == FlipKind.Horizontal)
                {
                    hintVertical.SetActive(player.flip.down.y == 0);
                    hintHorizontal.SetActive(player.flip.down.x == 0);
                }
            }
        }
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
                isFlipping = null;
                return;
            }
        }

        transform.rotation = q1;
        cancelFlip();
        transform.rotation = q0;

        foreach (var o in objects)
        {
            var dyn = o.GetComponentInChildren<MyDynamic>();
            if (dyn != null)
            {
                var (other, overlap) = Physics.AllCollisions(dyn).FirstOrDefault();

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
