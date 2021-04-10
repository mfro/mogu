using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenDeath : MonoBehaviour
{
    public LevelController controller;

    private MyCollider physics;

    void Awake()
    {
        Util.GetComponent(this, out physics);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!physics.enabled) return;

        var camera = controller.camera;

        var visible = Util.RectFromCenterSize(Physics.FromUnity(camera.transform.position), Physics.FromUnity(new Vector2(12, 12)));

        var overlap = Physics.Overlap(visible, physics.bounds);
        if (overlap != physics.bounds)
        {
            controller.DoDeath();
        }
    }
}
