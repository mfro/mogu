using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{
    private int updateRate;
    private int updateCount;
    private float updateTimer;

    private int fixedUpdateRate;
    private int fixedUpdateCount;
    private float fixedUpdateTimer;

    private float jumpStart;
    private float jumpHeight;

    private PlayerMovement player;
    private MyCollider playerPhysics => player.GetComponent<MyCollider>();

    private bool _onDebug = false;
    public void OnDebug(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onDebug)
            enabled = !enabled;

        _onDebug = c.ReadValueAsButton();
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(0, 0, 50, 20), $"{updateRate}");
        GUI.TextArea(new Rect(50, 0, 50, 20), $"{fixedUpdateRate}");
        GUI.TextArea(new Rect(0, 20, 50, 20), $"{playerPhysics.bounds.yMin}");
    }

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        updateCount += 1;
        if (Time.time - updateTimer >= 1)
        {
            updateRate = updateCount;
            updateCount = 0;
            updateTimer += 1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        fixedUpdateCount += 1;
        if (Time.time - fixedUpdateTimer >= 1)
        {
            fixedUpdateRate = fixedUpdateCount;
            fixedUpdateCount = 0;
            fixedUpdateTimer += 1;
        }

        if (player.jumping)
        {
            jumpStart = playerPhysics.position.y;
            jumpHeight = 0;
        }
        else
        {
            jumpHeight = Mathf.Max(jumpHeight, Mathf.Abs(playerPhysics.position.y - jumpStart));
        }
    }
}
