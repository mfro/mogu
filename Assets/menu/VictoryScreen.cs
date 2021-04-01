using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    // Update is called once per frame
    public void DoPlayAgain()
    {
        SceneManager.LoadScene(0);
    }
}
