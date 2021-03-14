using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CellFlip : MonoBehaviour
{
    private static Quaternion[,] rotations =
    {
        { Quaternion.identity, Quaternion.AngleAxis(180, Vector3.left) },
        { Quaternion.AngleAxis(90, Vector3.forward), Quaternion.AngleAxis(180, Vector3.left) * Quaternion.AngleAxis(-90, Vector3.forward) },
        { Quaternion.AngleAxis(180, Vector3.forward), Quaternion.AngleAxis(180, Vector3.left) * Quaternion.AngleAxis(180, Vector3.forward) },
        { Quaternion.AngleAxis(-90, Vector3.forward), Quaternion.AngleAxis(180, Vector3.left) * Quaternion.AngleAxis(90, Vector3.forward) },
    };

    [SerializeField]
    float fade_time = 1;

    [SerializeField]
    float flip_time = 1;

    [SerializeField]
    MeshRenderer cube;

    bool isFlipping = false;

    private Quaternion origin;
    public int rotation;
    public int flip;

    // Start is called before the first frame update
    private void Start()
    {
        origin = transform.localRotation;
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.DrawLine(transform.position, rotations[rotation, flip] * Vector2.down * 10, Color.red);
    }

    public async void DoFlip(int rotating, int flipping)
    {
        if (isFlipping) return;
        isFlipping = true;

        var allObjects = Resources.FindObjectsOfTypeAll<Flippable>();
        var x = Physics2D.OverlapBoxAll(transform.position, transform.lossyScale, 0);
        var parents = x.Select(o => o.transform.parent).ToArray();

        foreach (var o in x)
        {
            if (o.gameObject == gameObject)
                continue;

            o.transform.parent = transform;
            var f = o.GetComponent<Flippable>();
            if (f != null)
            {
                f.flipping = true;
            }
        }

        //        {
        //            Color c0 = new Color(0, 0, 0, 0);
        //            Color c1 = new Color(0.09f, 0.09f, 0.09f, 0.125f);
        //
        //            float t0 = Time.time;
        //            while (Time.time - t0 < fade_time)
        //            {
        //                cube.material.color = Color.Lerp(c0, c1, (Time.time - t0) / flip_time);
        //                await Task.Yield();
        //            }
        //            cube.material.color = c1;
        //        }

        rotation = (rotation + rotating) % rotations.GetLength(0);
        flip = (flip + flipping) % rotations.GetLength(1);

        var q0 = transform.localRotation;
        var q1 = origin * rotations[rotation, flip];

        float t0 = Time.time;
        while (Time.time - t0 < flip_time)
        {
            transform.localRotation = Quaternion.Lerp(q0, q1, (Time.time - t0) / flip_time);
            await Task.Yield();
        }

        transform.localRotation = rotations[rotation, flip];
        var down = rotations[rotation, flip] * Vector2.down;
        down.x = Mathf.Round(down.x);
        down.y = Mathf.Round(down.y);
        down.z = Mathf.Round(down.z);

        //        {
        //            Color c0 = new Color(0.09f, 0.09f, 0.09f, 0.125f);
        //            Color c1 = new Color(0, 0, 0, 0);
        //
        //            float t0 = Time.time;
        //            while (Time.time - t0 < fade_time)
        //            {
        //                cube.material.color = Color.Lerp(c0, c1, (Time.time - t0) / flip_time);
        //                await Task.Yield();
        //            }
        //            cube.material.color = c1;
        //        }

        foreach (var (o, parent) in x.Zip(parents, (l, r) => (l, r)))
        {
            if (o.gameObject == gameObject)
                continue;

            o.transform.parent = parent;
            var f = o.GetComponent<Flippable>();
            if (f != null)
            {
                f.SetDown(down);
                f.flipping = false;
            }
        }

        isFlipping = false;
    }
}
