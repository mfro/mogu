using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    async void Start()
    {
        await Util.Seconds(4, false);
        SceneController.instance.SwitchScene(1);
    }
}
