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
    static bool isFlipping = false;

    [SerializeField]
    public FlipKind flip1;

    [SerializeField]
    public FlipKind flip2;

    [SerializeField]
    float flip_time = 1;

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
        isFlipping = true;

        var levelController = FindObjectOfType<LevelController>();
        levelController?.SaveUndoState();

        var allObjects = Resources.FindObjectsOfTypeAll<Flippable>();

        var area = Physics.RectFromCenterSize(Physics.FromUnity(transform.position), Physics.FromUnity(transform.lossyScale));

        var x = Physics.AllOverlaps(area, CollideReason.CellFlip)
            .Where(o => o.Item2 == o.Item1.bounds)
            .Select(o => o.Item1.GetComponent<Flippable>())
            .Where(o => o != null);

        var parents = x.Select(o => o.transform.parent).ToArray();

        foreach (var o in x)
        {
            o.flipping = true;
        }

        foreach (var o in x)
        {
            o.transform.parent = transform;
            if (o != null) o.DoBeginFlip();
        }

        var q0 = transform.rotation;
        var q1 = delta * q0;

        float t0 = Time.time;
        while (Time.time - t0 < flip_time)
        {
            transform.rotation = Quaternion.Lerp(q0, q1, (Time.time - t0) / flip_time);
            await Task.Yield();
        }

        transform.rotation = q1;

        foreach (var (o, parent) in x.Zip(parents, (l, r) => (l, r)))
        {
            if (o.gameObject == gameObject)
                continue;

            o.transform.parent = parent;
            var f = o.GetComponent<Flippable>();
            if (f != null)
            {
                f.DoEndFlip(delta);
            }
        }

        transform.rotation = q0;

        isFlipping = false;
    }
}
