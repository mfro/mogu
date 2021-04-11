using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    //public bool encumbered;
    public Vector2 facing;

    [NonSerialized] public bool isDead;

    [NonSerialized] public Flippable flip;
    private SpriteRenderer sprite;
    private PlayerMovement movement;
    private Animator anim;
    private MyDynamic dyn;

    [SerializeField] AudioSource walkAudioSource;
    [SerializeField] AudioSource OneShotAudioSource;
    [SerializeField] AudioClip LandSound;
    [SerializeField] AudioClip JumpSound;

    private Vector2 input_movement;
    private bool previouslyGrounded = false;
    private bool previouslyJumping = false;
    private bool isPushing;

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
        anim.speed = (isDead || dyn.enabled) ? 1 : 0;

        previouslyGrounded = dyn.grounded;
        previouslyJumping = movement.jumping;

        if (dyn.enabled)
        {
            // Debug.DrawLine(transform.position, transform.position + (transform.rotation * Vector2.right), Color.red);
            // Debug.DrawLine(transform.position, transform.position + (Vector3)facing, Color.blue);
            // Debug.DrawLine(transform.position, transform.position + (Vector3)dyn.velocity, Color.green);

            isPushing = dyn.grounded
                && Vector2.Dot(facing, dyn.velocity) > 0
                && Physics.AllCollisions(dyn.bounds.Shift(facing), dyn.mask).Where(c => c.Item1 != dyn).Any();

            anim.SetBool("pushing", isPushing);

            var spriteRight = (transform.rotation * Vector2.right);
            sprite.flipX = (Vector2.Dot(spriteRight, facing) < 0);

            anim.SetFloat("running speed", Mathf.Abs(movement.input_running));
        }

        if (dyn.enabled && dyn.grounded)
        {
            if (!previouslyGrounded)
            {
                OneShotAudioSource.PlayOneShot(LandSound);
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

        walkAudioSource.pitch = isPushing ? 0.6f : 1;

    }


    public void PlayJumpSound()
    {
        OneShotAudioSource.Stop();
        OneShotAudioSource.PlayOneShot(JumpSound);
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
    }

    public void Move(InputAction.CallbackContext callback)
    {
        var isKeyboard = callback.control.device is Keyboard;
        var mode = isKeyboard ? InputMode.Keyboard : InputMode.Controller;

        if (mode != Hint.mode)
        {
            Hint.mode = mode;
            foreach (var hint in FindObjectsOfType<Hint>())
            {
                hint.ReRender();
            }
        }

        input_movement = Util.Round(callback.ReadValue<Vector2>());
        UpdateMovement();
    }

    public void Jump(InputAction.CallbackContext callback)
    {
        if (Physics.IsEnabled) movement.input_jumping = callback.ReadValueAsButton();
    }

    public void DoFlip(int input)
    {
        if (!dyn.enabled || !dyn.grounded)
            return;

        foreach (var cell in FindObjectsOfType<FlipPanel>())
        {
            var overlap = Physics.Overlap(dyn.bounds, cell.physics.bounds);
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
