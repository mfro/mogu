using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public UnityEvent OnKillEvent = new UnityEvent();
    LevelController levelController;

    private void Awake()
    {
        levelController = transform.parent.GetComponent<LevelController>();
    }

    public void KillPlayer()
    {
        OnKillEvent.Invoke();
        levelController.DoDeath();
    }
}
