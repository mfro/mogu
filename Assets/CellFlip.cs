using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CellFlip : MonoBehaviour
{
    [SerializeField]
    float fade_time = 1;

    [SerializeField]
    float flip_time = 1;

    [SerializeField]
    MeshRenderer cube;

    bool isFlipping = false;

    private Quaternion[] rotations;
    private int rotation;

    // Start is called before the first frame update
    private void Start()
    {
        rotations = new Quaternion[2];
        rotations[0] = transform.localRotation;
        rotations[1] = transform.localRotation * Quaternion.Euler(180, 0, 0);
    }

    // Update is called once per frame
    private void Update()
    {

    }

    public async void DoFlip()
    {
        if (isFlipping) return;
        isFlipping = true;

        var allObjects = Resources.FindObjectsOfTypeAll<Flippable>();
        var x = Physics.OverlapBox(transform.position, new Vector3(5, 5, 1000000));
        var parents = x.Select(o => o.transform.parent).ToArray();

        foreach (var o in x)
        {
            if (o.gameObject == gameObject)
                continue;

            o.transform.parent = transform;
            var f = o.GetComponent<Flippable>();
            if (f != null)
            {
                f.DoFlip();
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

        {
            var r0 = rotations[rotation];
            rotation = (rotation + 1) % rotations.Length;
            var r1 = rotations[rotation];

            float t0 = Time.time;
            while (Time.time - t0 < flip_time)
            {
                transform.localRotation = Quaternion.Lerp(r0, r1, (Time.time - t0) / flip_time);
                await Task.Yield();
            }
            transform.localRotation = r1;
        }

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

        foreach (var (o, parent) in x.Zip(parents, (a, b) => (a, b)))
        {
            if (o.gameObject == gameObject)
                continue;

            o.transform.parent = parent;
            var f = o.GetComponent<Flippable>();
            if (f != null)
            {
                f.flipping = false;
            }
        }

        isFlipping = false;
    }
}
