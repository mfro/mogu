using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void OnOptions()
    {
        print("options");
    }

    public void OnCredits()
    {
        print("credits");
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
