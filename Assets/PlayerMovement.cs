using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float MoveSpeed = 10;

    Vector2 PendingInput;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 velocity = rb.velocity;
        velocity.x = PendingInput.x * MoveSpeed;
        rb.velocity = velocity;
    }

    public void Move(InputAction.CallbackContext callback)
    {
        PendingInput = callback.ReadValue<Vector2>();
    }
}
