using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//COMPORTAMENT PER GESTIONAR LA VIDA
public class Health : MonoBehaviour
{
    public float currentHealth = 100f;
    public float maxHealth = 100f;

    public float currentShield = 100f;
    public float maxShield = 100f;
    public float shieldDamage = 0.9f;

    public RagdollController ragdoll;

    //public Slider shieldSlider, healthSlider;

    public UnityEvent takeDamageEvent, DieEvent;

    void Start(){
        //shieldSlider.value = 100f;
        //healthSlider.value = 100f; 
    }

    //Mètode per treure vida
    public bool TakeDamage(float damage, Rigidbody hitRb, Vector3 force){
        if(currentShield > 0f){ //Treiem escut en cas que en tinguem
            currentShield -= damage * shieldDamage;
            currentHealth -= damage * (1-shieldDamage);

            if(currentShield < 0f) currentShield = 0f;
            if(currentHealth < 0f) currentHealth = 0f;
        }
        else{ //En cas que no tinguem escut treiem vida
            currentHealth -= damage;
            if(currentHealth <= 0f){
                currentHealth = 0f;
                Mort(hitRb, force);
                return true;
            }
        }

        takeDamageEvent.Invoke();
        return false;

        //shieldSlider.value = currentShield;
        //healthSlider.value = currentHealth;
    }

    //Curem
    public void Healing(float healAmaount){
        if(currentHealth < maxHealth){
            currentHealth += healAmaount;
            if(currentHealth > maxHealth) currentHealth = maxHealth;
        }

        //healthSlider.value = currentHealth;
    }

    //Recuperem escut
    public void HealingShield(float healAmaount){
        if(currentShield < maxShield){
            currentShield += healAmaount;
            if(currentShield > maxShield) currentShield = maxShield;
        }

        //shieldSlider.value = currentShield;
    }

    void Mort(Rigidbody hitRb, Vector3 force){
        DieEvent.Invoke();
        if(this.gameObject.tag == "Enemy" || this.gameObject.layer == 10){
   
            SetLayerRecursively(this.gameObject, 16);
            
            // SI es tracta d'un zombi desactivem moviment
            if(GetComponent<UnityEngine.AI.NavMeshAgent>() != null) GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            if(GetComponent<ZombieAI>() != null){
                GetComponent<ZombieAI>().enabled = false;
                GameObject.FindGameObjectWithTag("MainGame").GetComponent<MainGame>().ValidarEnemicsVius();
            }

            // Activem ragdoll
            if(ragdoll != null) ragdoll.SetRagdoll(true);

            // DESPRÉS APLICA FORÇA
            if(hitRb != null)
            {
                hitRb.AddForce(force, ForceMode.Impulse);
            }

            //Destroy(this.gameObject);
        }
        else if(this.gameObject.tag == "Player"){
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
