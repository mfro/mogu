using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OffScreenDeath : MonoBehaviour
{
    public LevelController controller;

    // Update is called once per frame
    void FixedUpdate()
    {
        var camera = controller.camera;

        var distance = (Vector2)camera.transform.position - (Vector2)transform.position;
        if (Mathf.Abs(distance.x) > camera.orthographicSize + 0.5 || Mathf.Abs(distance.y) > camera.orthographicSize + 0.5)
        {
            controller.DoDeath();
        }
    }
}
