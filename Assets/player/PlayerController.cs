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

    private PlayerMovement playerMovement;
    private Flippable flip;
    private Animator anim;
    private SpriteRenderer sprite;
    private MyCollider physics;

    private bool slipping = false;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        flip = GetComponent<Flippable>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        physics = GetComponent<MyCollider>();

        facing = Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + (transform.rotation * Vector2.right), Color.red);
        Debug.DrawLine(transform.position, transform.position + (Vector3)facing, Color.blue);

        anim.SetBool("grounded", physics.grounded);
        if (flip.flipping)
            anim.speed = 0;
        else
            anim.speed = 1;

        bool found = false;
        if (physics.grounded)
        {
            var results = Physics.BoxCast(physics.bounds, flip.down);
            foreach (var result in results)
            {
                if (result.Item1.gameObject.CompareTag("Slippery"))
                {
                    playerMovement.enabled = false;

                    slipping = true;
                    found = true;
                    break;
                }
            }
        }

        if (slipping)
        {
            var vel = Physics.Round(physics.velocity);

            if (!found || vel == Vector2.zero)
            {
                playerMovement.enabled = true;
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

    public void Move(InputAction.CallbackContext callback)
    {
        playerMovement.movement_input = callback.ReadValue<Vector2>();
        anim.SetFloat("running speed", Mathf.Abs(playerMovement.movement_input.x));

        var spriteRight = (transform.rotation * Vector2.right);

        if (playerMovement.movement_input != Vector2.zero)
        {
            if (flip.down.x == 0)
                facing = MatchFacing(playerMovement.movement_input.x, Vector2.left, Vector2.right);
            else if (playerMovement.movement_input.y != 0)
                facing = MatchFacing(playerMovement.movement_input.y, Vector2.down, Vector2.up);
            else if (flip.down == Vector2.right)
                facing = MatchFacing(playerMovement.movement_input.x, Vector2.down, Vector2.up);
            else
                facing = MatchFacing(playerMovement.movement_input.x, Vector2.up, Vector2.down);
        }

        sprite.flipX = (Vector2.Dot(spriteRight, facing) < 0);

        if (playerMovement.movement_input.y != 0 && flip.down.x != 0)
            anim.SetFloat("running speed", Mathf.Abs(playerMovement.movement_input.y));
    }

    public void Jump(InputAction.CallbackContext callback)
    {
        playerMovement.jumping_input = callback.ReadValueAsButton();
    }

    public void DoFlip(int input)
    {
        if (!physics.grounded || slipping)
            return;

        var cell = FindObjectsOfType<CellFlip>();
        var closest = cell.OrderBy(o => (o.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
        if (closest == null)
            return;

        var area = Physics.RectFromCenterSize(Physics.FromUnity(closest.transform.position), Physics.FromUnity(closest.transform.lossyScale));
        var overlap = Physics.Overlap(physics.bounds, area);

        if (overlap != null && overlap.Value == physics.bounds)
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
