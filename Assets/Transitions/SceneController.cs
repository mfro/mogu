using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public Animator transitionAnim;
    public float transitionDuration;

    public static SceneController sceneController;


    private int nextLevel = 0;

    // Start is called before the first frame update
    void Awake()
    {
        if (sceneController == null)
        {
            sceneController = this;
            DontDestroyOnLoad(this);
        } 
        else
        {
            Destroy(this.gameObject);
        }

    }

    public void SwitchScene(int index)
    {
        nextLevel = index;
        transitionAnim.SetTrigger("Start");
    }

    private void EndSceneSwitch()
    {
        SceneManager.LoadScene(nextLevel);
        transitionAnim.SetTrigger("End");
    }
    
}
