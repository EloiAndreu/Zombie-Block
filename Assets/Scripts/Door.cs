using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool open = false;
    public bool defaultSatateOpen = false;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ChangeDoorState()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if(stateInfo.normalizedTime < 1f)  return;

        open = !open;
        

        if (open)
        {
            anim.SetBool("Open", true);
        }
        else
        {
            anim.SetBool("Open", false);
        }
    }
}
