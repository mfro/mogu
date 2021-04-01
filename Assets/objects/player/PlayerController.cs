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

    private SpriteRenderer sprite;
    private PlayerMovement movement;
    private Flippable flip;
    private Animator anim;
    private MyDynamic dyn;

    [SerializeField] AudioSource walkAudioSource;
    [SerializeField] AudioSource landAudioSource;
    [SerializeField] AudioClip LandSound;

    private Vector2 input_movement;
    private bool previouslyGrounded = false;

    void Awake()
    {
        Util.GetComponentInChildren(this, out sprite);
        Util.GetComponent(this, out movement);
        Util.GetComponent(this, out flip);
        Util.GetComponent(this, out anim);
        Util.GetComponent(this, out dyn);
    }

    void Start()
    {
        dyn.mask |= CollisionMask.Player;

        facing = Vector2.right;

        Time.fixedDeltaTime = 1 / 60f;

        flip.EndFlip += (delta) =>
        {
            facing = delta * facing;
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

        if (dyn.grounded && !flip.flipping)
        {
            if (!previouslyGrounded)
            {
                landAudioSource.PlayOneShot(LandSound);
            }
            if (dyn.velocity.magnitude > 0)
            {
                if (!walkAudioSource.isPlaying) walkAudioSource.Play();
            }
            else
            {
                if (walkAudioSource.isPlaying) walkAudioSource.Pause();
            }
        }
        else
        {
            if (walkAudioSource.isPlaying) walkAudioSource.Pause();
        }

        previouslyGrounded = dyn.grounded;

        // Debug.DrawLine(transform.position, transform.position + (transform.rotation * Vector2.right), Color.red);
        // Debug.DrawLine(transform.position, transform.position + (Vector3)facing, Color.blue);
        // Debug.DrawLine(transform.position, transform.position + (Vector3)dyn.velocity, Color.green);

        var isPushing = dyn.grounded
            && Vector2.Dot(facing, dyn.velocity) > 0
            && Physics.AllCollisions(dyn.bounds.Shift(facing), dyn.mask).Where(c => c.Item1 != dyn).Any();

        anim.SetBool("pushing", isPushing);

        // UnityEditor.Selection.instanceIDs = dyn.touching.Select(o => o.gameObject.GetInstanceID()).ToArray();
    }

    private static Vector2 MatchFacing(float value, Vector2 negative, Vector2 positive)
    {
        if (value < 0) return negative;
        if (value > 0) return positive;
        return Vector2.zero;
    }

    public void UpdateMovement()
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
        if (!dyn.grounded)
            return;

        foreach (var cell in FindObjectsOfType<CellFlip>())
        {
            var area = Physics.RectFromCenterSize(Physics.FromUnity(cell.transform.position), Physics.FromUnity(cell.transform.lossyScale));
            var overlap = Physics.Overlap(dyn.bounds, area);

            if (overlap == null) continue;

            if (overlap.Value == dyn.bounds)
            {
                cell.DoFlip(flip.down, input);
            }
            else
            {
                cell.ShowFlipError(overlap.Value);
            }

            return;
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
