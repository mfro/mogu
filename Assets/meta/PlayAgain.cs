using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayAgain : MonoBehaviour
{
    private bool playedAgain = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void DoPlayAgain(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !playedAgain)
        {
            playedAgain = true;
            SceneManager.LoadScene(0);
        }
    }
}
