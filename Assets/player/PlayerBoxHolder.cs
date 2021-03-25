using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBoxHolder : MonoBehaviour
{
    private PlayerController controller;
    private Flippable flip;

    public Pushable holding;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        flip = GetComponent<Flippable>();

        holding = null;
    }

    public bool TryInteract()
    {
        // var offset = controller.facing * 0.5f;
        // var size = new Vector2(0.5f, 0.5f);

        // var collisions = Physics2D.OverlapBoxAll((Vector2)transform.position + offset, size, 0);
        // var boxes = collisions.Select(o => o.GetComponent<Pushable>()).Where(o => o != null);

        // var box = boxes.FirstOrDefault();
        // if (box == null)
        //     return false;

        // holding = box;
        // box.GetComponent<Rigidbody2D>().isKinematic = true;

        // box.transform.SetParent(transform);
        // box.transform.localPosition = offset;
        // // box.GetComponent<BoxCollider2D>().

        return true;
    }
}
