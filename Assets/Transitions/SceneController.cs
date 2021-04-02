using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public Animator transitionAnim;

    public static SceneController sceneController;

    public static int NextLevel => SceneManager.GetActiveScene().buildIndex + 1;

    [SerializeField] bool fadeOnInit;

    private Scene? doneLoad;
    private bool doneAnim = false;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SwitchScene(int index, float duration = 1f)
    {
        transitionAnim.enabled = true;
        transitionAnim.speed = 1 / duration;
        transitionAnim.SetTrigger("Start");

        doneLoad = null;
        doneAnim = false;

        SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
    }

    private void Finish()
    {
        if (!doneAnim || doneLoad == null) return;

        var from = SceneManager.GetActiveScene();
        var to = doneLoad.Value;

        foreach (var obj in from.GetRootGameObjects())
            obj.SetActive(false);

        foreach (var obj in to.GetRootGameObjects())
            obj.SetActive(true);

        SceneManager.UnloadSceneAsync(from);
        SceneManager.SetActiveScene(to);

        transitionAnim.SetTrigger("End");
    }

    private void EndSceneSwitch()
    {
        doneAnim = true;
        Finish();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Additive) return;

        foreach (var obj in scene.GetRootGameObjects())
            obj.SetActive(false);

        doneLoad = scene;
        Finish();
    }
}
