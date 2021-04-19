using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class VictoryScreen2 : MonoBehaviour
{
    public void DoQuit()
    {
        SceneController.instance.SwitchScene(2);
    }
}
