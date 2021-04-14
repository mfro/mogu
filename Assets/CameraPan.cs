using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPan : MonoBehaviour
{

    public Vector3 EndPos;
    public float PanTime = 5;
    public Camera Main;
    public Camera Panels;
    public float EndSize = 15;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MakeCameraMove(InputAction.CallbackContext c)
    {
        if(c.ReadValueAsButton())
        {
            StartCoroutine(MoveCamera());
        }
    }

    IEnumerator MoveCamera()
    {
        float StartTime = Time.time;
        float StartSize = Main.orthographicSize;
        Vector3 StartPos = transform.position;
        while(StartTime + PanTime >= Time.time)
        {
            transform.position = Vector3.Lerp(StartPos, EndPos, (Time.time - StartTime) / PanTime);
            Main.orthographicSize = Mathf.Lerp(StartSize, EndSize, (Time.time - StartTime) / PanTime);
            Panels.orthographicSize = Main.orthographicSize;
            yield return null;
        }
    }
}
