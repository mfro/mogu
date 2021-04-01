using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public Animator transitionAnim;
    public float transitionDuration;

    public static SceneController sceneController;

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

    private IEnumerator SwitchSceneRoutine(int index)
    {
        transitionAnim.SetTrigger("Start");
        yield return new WaitForSeconds(transitionDuration);
        transitionAnim.SetTrigger("End");
        SceneManager.LoadScene(index);
    }

    public void SwitchScene(int index)
    {
        StartCoroutine(SwitchSceneRoutine(index));
    }
}
