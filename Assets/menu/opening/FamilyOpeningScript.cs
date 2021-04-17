using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyOpeningScript : MonoBehaviour
{
    [SerializeField] Animator[] familyAnims;

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
        }
    }
}
