using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    PlayerMovement playerMovement;
    private Rigidbody2D body;
    private new BoxCollider2D collider;
    private Flippable flippable;
    private Animator anim;
    private SpriteRenderer sprite;
    private PhysicsObject physics;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.enabled = true;
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        flippable = GetComponent<Flippable>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        physics = GetComponent<PhysicsObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + (transform.rotation * Vector2.right), Color.red);

        Vector2 face;
        if (flippable.down.x == 0) face = facing(playerMovement.movement_input.x, Vector2.left, Vector2.right);
        else
        {
            if (playerMovement.movement_input.y != 0)
                face = facing(playerMovement.movement_input.y, Vector2.down, Vector2.up);
            else if (flippable.down == Vector2.right)
                face = facing(playerMovement.movement_input.x, Vector2.down, Vector2.up);
            else
                face = facing(playerMovement.movement_input.x, Vector2.up, Vector2.down);
        }

        if (face != Vector2.zero)
            Debug.DrawLine(transform.position, transform.position + (Vector3)face, Color.blue);

        anim.SetBool("grounded", physics.grounded);
        if (flippable.flipping)
            anim.speed = 0;
        else
            anim.speed = 1;
    }

    private static Vector2 facing(float value, Vector2 negative, Vector2 positive)
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
        Vector2 face;

        if (flippable.down.x == 0) face = facing(playerMovement.movement_input.x, Vector2.left, Vector2.right);
        else
        {
            if (playerMovement.movement_input.y != 0)
                face = facing(playerMovement.movement_input.y, Vector2.down, Vector2.up);
            else if (flippable.down == Vector2.right)
                face = facing(playerMovement.movement_input.x, Vector2.down, Vector2.up);
            else
                face = facing(playerMovement.movement_input.x, Vector2.up, Vector2.down);
        }

        if (face != Vector2.zero)
        {
            Debug.DrawLine(transform.position, transform.position + (Vector3)face, Color.blue);
            sprite.flipX = (Vector2.Dot(spriteRight, face) < 0);
        }

        if (playerMovement.movement_input.y != 0 && flippable.down.x != 0)
            anim.SetFloat("running speed", Mathf.Abs(playerMovement.movement_input.y));
    }

    public void Jump(InputAction.CallbackContext callback)
    {
        playerMovement.jumping_input = callback.ReadValueAsButton();
    }

    public void DoFlip(int input)
    {
        var cell = FindObjectsOfType<CellFlip>();
        var closest = cell.OrderBy(o => (o.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
        if (closest == null)
            return;

        var contained = collider.bounds.min.x >= closest.transform.position.x - closest.transform.lossyScale.x / 2
            && collider.bounds.max.x <= closest.transform.position.x + closest.transform.lossyScale.x / 2
            && collider.bounds.min.y >= closest.transform.position.y - closest.transform.lossyScale.y / 2
            && collider.bounds.max.y <= closest.transform.position.y + closest.transform.lossyScale.y / 2;

        if (contained)
        {
            closest.DoFlip(flippable.down, input);
        }
    }

    public void OnFlip1(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton())
            DoFlip(1);
    }

    public void OnFlip2(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton())
            DoFlip(2);
    }
}
