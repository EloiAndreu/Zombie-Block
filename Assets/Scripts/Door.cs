using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool open = false;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ChangeDoorState()
    {
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
