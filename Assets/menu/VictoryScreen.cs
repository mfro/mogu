using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreen : MonoBehaviour
{
    // Update is called once per frame
    public void DoPlayAgain()
    {
        SceneController.sceneController.SwitchScene(0);
    }
}
