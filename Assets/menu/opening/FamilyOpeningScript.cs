using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyOpeningScript : MonoBehaviour
{
    [SerializeField] Animator[] familyAnims;


    [SerializeField] Animator backgroundAnim;

    private bool started = false;
    private bool finished = false;

    void Start()
    {
        foreach (var anim in familyAnims)
        {
            anim.speed = 0f;
        }
    }


    public void WakeupFamily()
    {
        foreach (var anim in familyAnims)
        {
            anim.speed = 1f;
            started = true;
        }
    }

    public void Update()
    {
        if (!started || finished) return;

        foreach (var anim in familyAnims)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            {
                print("all good");
                continue;
            }
            else
            {
                return;
            }
        }

        finished = true;
        backgroundAnim.SetTrigger("FinishRising");
    }
}
