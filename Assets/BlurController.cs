using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurController : MonoBehaviour
{

    [SerializeField] ScreenBlur[] blurs;

    public static BlurController instance;

    private bool isBlurred;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        foreach (var blur in blurs)
        {
            blur.enabled = false;
        }

        isBlurred = false;
    }

    public void SetBlursEnabled(bool enabled)
    {
        foreach (var blur in blurs)
        {
            blur.enabled = enabled;
        }
        isBlurred = enabled;
    }
}
