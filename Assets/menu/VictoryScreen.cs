using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreen : MonoBehaviour
{
    public void DoPlayAgain()
    {
        SceneController.sceneController.SwitchScene(0);
    }
}
