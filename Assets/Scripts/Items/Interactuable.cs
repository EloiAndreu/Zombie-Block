using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//COMPORTAMENT DELS OBJECTES AGAFABLES
public class Interactuable : MonoBehaviour
{
    public bool desactivarAnimacióAlPrincipi = true;
    public Animator anim;
    public List<UnityEvent> interactuarEvent; 

    void Start(){
        if(anim != null && desactivarAnimacióAlPrincipi) anim.enabled = false;
    }

    //En prèmer el botó esquerra del ratolí es cridarà aquest mètode
    //Permet implemetar diferents funcionaltiats segons l'objecte
    //Ho fem a través de UnityEvents
    public void Interactua(int id){
        if(interactuarEvent.Count > id && id >= 0) interactuarEvent[id].Invoke();
    }
}
