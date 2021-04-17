using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOpeningScript : MonoBehaviour
{

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        animator.speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AwakenPlayer()
    {
        animator.speed = 1f;
    }


    public void PlayerFinishedWalkingOff()
    {
        print("scene done");
    }

    public void PlayerFinishedWakingUp()
    {
        animator.SetTrigger("Start Walking");
        animator.speed = 1.1f;
    }
}
