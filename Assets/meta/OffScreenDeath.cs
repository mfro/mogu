using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OffScreenDeath : MonoBehaviour
{
    public new Camera camera;

    // Update is called once per frame
    void Update()
    {
        var distance = (Vector2)camera.transform.position - (Vector2)transform.position;
        if (distance.magnitude > 2 * camera.orthographicSize)
        {
            LevelManager.Restart();
        }
    }
}
