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

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.enabled = true;
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        flippable = GetComponent<Flippable>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Move(InputAction.CallbackContext callback)
    {
        playerMovement.movement_input = callback.ReadValue<Vector2>();
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

    public void OnRestart(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton())
            LevelManager.Restart();
    }
}
