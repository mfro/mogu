using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundOpeningScript : MonoBehaviour
{
    [SerializeField] float initialDelay = 1f;
    private float currTime = 0f;
    private bool started = false;
    private Animator animator;

    [SerializeField] FamilyOpeningScript family;

    // Start is called before the first frame update
    void Start()
    {
        currTime = 0f;
        started = false;
        animator = GetComponent<Animator>();
        animator.speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (started) return;
        currTime += Time.deltaTime;
        while (currTime < initialDelay && !started)
        {
            return;
        }

        started = true;
        animator.speed = 0.5f;
    }

    public void TriggerFamilyWakeup()
    {
        family.WakeupFamily();
    }
}
