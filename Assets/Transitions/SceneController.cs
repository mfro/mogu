using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static event Action SceneChanged;

    public static SceneController sceneController;

    public static int NextLevel => SceneManager.GetActiveScene().buildIndex + 1;

    public Animator transitionAnim;

    [SerializeField] bool fadeOnInit;

    private int loadingScene;

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

        if (fadeOnInit)
        {
            transitionAnim.enabled = true;
        }
        else
        {
            transitionAnim.enabled = false;
        }
    }

    public void SwitchScene(int index, float duration = 0.75f)
    {
        loadingScene = index;
        transitionAnim.enabled = true;
        transitionAnim.speed = 1 / duration;
        transitionAnim.SetTrigger("Start");
    }

    private void EndSceneSwitch()
    {
        SceneManager.LoadScene(loadingScene);
        SceneChanged?.Invoke();
        transitionAnim.SetTrigger("End");
    }
}
