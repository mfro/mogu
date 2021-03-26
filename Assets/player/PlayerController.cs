using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    //public bool encumbered;
    public Vector2 facing;

    private PlayerMovement movement;
    private Flippable flip;
    private Animator anim;
    private MyDynamic dyn;
    private SpriteRenderer sprite;

    private bool slipping = false;

    private Vector2 input_movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        flip = GetComponent<Flippable>();
        anim = GetComponent<Animator>();
        dyn = GetComponent<MyDynamic>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        facing = Vector2.right;

        Time.fixedDeltaTime = 1 / 60f;

        flip.EndFlip += (delta) =>
        {
            UpdateMovement();
        };
    }

    void Update()
    {
        anim.SetBool("grounded", dyn.grounded);
        if (flip.flipping)
            anim.speed = 0;
        else
            anim.speed = 1;
    }

    void FixedUpdate()
    {
        // Debug.DrawLine(transform.position, transform.position + (transform.rotation * Vector2.right), Color.red);
        // Debug.DrawLine(transform.position, transform.position + (Vector3)facing, Color.blue);

        bool found = false;
        if (dyn.grounded)
        {
            slipping = Physics.AllOverlaps(CollisionMask.Physical, dyn.bounds.Shift(flip.down))
                .Where(c => c.Item1.CompareTag("Slippery"))
                .Any();

            if (slipping)
            {
                movement.enabled = false;
                found = true;
            }
        }

        if (slipping)
        {
            var vel = Physics.Round(dyn.velocity);

            if (!found || vel == Vector2.zero)
            {
                movement.enabled = true;
                slipping = false;
            }
        }
    }

    private static Vector2 MatchFacing(float value, Vector2 negative, Vector2 positive)
    {
        if (value < 0) return negative;
        if (value > 0) return positive;
        return Vector2.zero;
    }

    private void UpdateMovement()
    {
        if (flip.down.x != 0 && input_movement.x == 0)
        {
            movement.input_running = input_movement.y;
        }
        else
        {
            movement.input_running = input_movement.x * Mathf.Sign(flip.down.x);
        }

        if (movement.input_running != 0)
        {
            if (flip.down.x == 0)
                facing = MatchFacing(movement.input_running, Vector2.left, Vector2.right);
            else
                facing = MatchFacing(movement.input_running, Vector2.down, Vector2.up);
        }

        var spriteRight = (transform.rotation * Vector2.right);
        sprite.flipX = (Vector2.Dot(spriteRight, facing) < 0);

        anim.SetFloat("running speed", Mathf.Abs(movement.input_running));
    }

    public void Move(InputAction.CallbackContext callback)
    {
        input_movement = callback.ReadValue<Vector2>();
        if (!flip.flipping)
            UpdateMovement();
    }

    public void Jump(InputAction.CallbackContext callback)
    {
        movement.input_jumping = callback.ReadValueAsButton();
    }

    public void DoFlip(int input)
    {
        if (!dyn.grounded || slipping)
            return;

        var cell = FindObjectsOfType<CellFlip>();
        var closest = cell.OrderBy(o => (o.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
        if (closest == null)
            return;

        var area = Physics.RectFromCenterSize(Physics.FromUnity(closest.transform.position), Physics.FromUnity(closest.transform.lossyScale));
        var overlap = Physics.Overlap(dyn.bounds, area);

        if (overlap != null && overlap.Value == dyn.bounds)
        {
            closest.DoFlip(flip.down, input);
        }
    }

    private bool _OnInteract = false;
    public void OnInteract(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_OnInteract)
        {
        }

        _OnInteract = c.ReadValueAsButton();
    }

    private bool _OnFlip1 = false;
    public void OnFlip1(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_OnFlip1)
            DoFlip(1);

        _OnFlip1 = c.ReadValueAsButton();
    }

    private bool _OnFlip2 = false;
    public void OnFlip2(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_OnFlip2)
            DoFlip(2);

        _OnFlip2 = c.ReadValueAsButton();
    }
}
