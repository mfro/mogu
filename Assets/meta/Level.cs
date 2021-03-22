using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject start;
    public Victory end;

    public void Start() {
        LevelManager.LoadLevel(this);
    }
}
