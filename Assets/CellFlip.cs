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
    bool can_flip = false;

    [SerializeField]
    bool can_rotate = false;

    [SerializeField]
    float flip_time = 1;

    public async void DoFlip(Vector2 down, FlipKind flip)
    {
        Quaternion delta;
        if (flip == FlipKind.CW)
        {
            if (!can_rotate) return;
            delta = Quaternion.AngleAxis(90, Vector3.back);
        }
        else if (flip == FlipKind.CCW)
        {
            if (!can_rotate) return;
            delta = Quaternion.AngleAxis(-90, Vector3.back);
        }
        else if (flip == FlipKind.Horizontal)
        {
            if (!can_flip) return;
            delta = Quaternion.AngleAxis(180, down);
        }
        else if (flip == FlipKind.Vertical)
        {
            if (!can_flip) return;
            var axis = Quaternion.AngleAxis(90, Vector3.back) * down;
            delta = Quaternion.AngleAxis(180, axis);
        }
        else return;
 
        if (isFlipping) return;
        isFlipping = true;

        var allObjects = Resources.FindObjectsOfTypeAll<Flippable>();
        var x = Physics2D.OverlapBoxAll(transform.position, transform.lossyScale * 0.9f, 0);
        var parents = x.Select(o => o.transform.parent).ToArray();

        foreach (var o in x)
        {
            if (o.gameObject == gameObject)
                continue;

            o.transform.parent = transform;
            var f = o.GetComponent<Flippable>();
            if (f != null)
            {
                f.DoBeginFlip();
            }
        }

        var q0 = transform.localRotation;
        var q1 = delta * q0;

        float t0 = Time.time;
        while (Time.time - t0 < flip_time)
        {
            transform.localRotation = Quaternion.Lerp(q0, q1, (Time.time - t0) / flip_time);
            await Task.Yield();
        }

        transform.localRotation = q1;

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

        transform.localRotation = q0;

        isFlipping = false;
    }
}
