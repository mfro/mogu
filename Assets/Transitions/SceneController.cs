using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public Animator transitionAnim;

    public static SceneController sceneController;

    [SerializeField] bool fadeOnInit;

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
            return;
        }

    }

    public void SwitchScene(int index, float duration = 1f)
    {
        transitionAnim.enabled = true;
        nextLevel = index;
        transitionAnim.speed = 1 / duration;
        transitionAnim.SetTrigger("Start");
    }

    private void EndSceneSwitch()
    {
        SceneManager.LoadScene(nextLevel);
        transitionAnim.SetTrigger("End");
    }

}
