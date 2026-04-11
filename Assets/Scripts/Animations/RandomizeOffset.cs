using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeOffset : MonoBehaviour
{
    public string nameAnimation;
    public bool randomRotation = false;
    public bool randomScale = true;
    public float minSacale = 0.75f, maxScale = 1f;
    

    void Start()
    {
        /*if(nameAnimation != ""){
            Animator anim = GetComponent<Animator>();
            float randomOffSet = Random.Range(0f, 1f);
            anim.Play(nameAnimation, 0, randomOffSet);

            //float randomSpeed = Random.Range(0f, 1f);
            //anim.speed = randomSpeed;
        }*/

        if(randomScale){
            float randomScale = Random.Range(minSacale, maxScale);
            this.gameObject.transform.localScale *= randomScale;
        }

        if(randomRotation){
            float randomYRotation = Random.Range(0f, 360f);
            Quaternion newRotation = Quaternion.Euler(0f, randomYRotation, 0f);
            this.gameObject.transform.rotation = newRotation;
        }
    }
}
