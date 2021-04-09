using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
#if !UNITY_WEBGL
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void ResizeWindow()
    {
        var minGameSize = new Vector2(384, 384);
        var screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        var scale = screenSize / minGameSize;

        var maxScale = (int)Mathf.Min(scale.x, scale.y);

        var maxGameSize = maxScale * minGameSize;

        Screen.SetResolution((int)maxGameSize.x, (int)maxGameSize.y, FullScreenMode.Windowed);
    }
#endif
}
