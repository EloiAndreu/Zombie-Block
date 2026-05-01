using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyGiveObject : MonoBehaviour
{
    GameObject player;
    public GameObject boyPatrol, boyQuestion;
    public KeyCode lootingKey1 = KeyCode.Alpha1;
    //public KeyCode lootingKey2 = KeyCode.Alpha2;
    public GameObject objPosition;
    GameObject objGenerated;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SetObjectHand(GameObject objToSpawn)
    {
        objGenerated = Instantiate(objToSpawn, objPosition.transform.position, objPosition.transform.rotation, objPosition.transform);
        objGenerated.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update()
    {
        transform.LookAt(new Vector3(player.transform.position.x, 0f, player.transform.position.z));

        if(Vector3.Distance(this.transform.position, player.transform.position) > 15f)
        {
            if(boyPatrol != null)
            {
                Instantiate(boyQuestion, transform.position, transform.rotation);
                if(objGenerated != null)
                {
                    objGenerated.GetComponent<Rigidbody>().isKinematic = false;
                    Instantiate(objGenerated, objPosition.transform.position, objPosition.transform.rotation); 
                }
                Destroy(this.gameObject);
            }
        }
        else
        {
            MyInput();
        }
    }

    void MyInput(){ //Inputs
        /*if(Input.GetKeyDown (lootingKey2)){
            if(boyPatrol != null)
            {
                Instantiate(boyPatrol, transform.position, transform.rotation);
                objGenerated.GetComponent<Rigidbody>().isKinematic = false;
                Instantiate(objGenerated, objPosition.transform.position, objPosition.transform.rotation);
                Destroy(this.gameObject);
            }
        }*/

        if(Input.GetKeyDown (lootingKey1)){
            if(boyQuestion != null)
            {
                Instantiate(boyQuestion, transform.position, transform.rotation);
                if(objGenerated != null)
                {
                    objGenerated.GetComponent<Rigidbody>().isKinematic = false;
                    Instantiate(objGenerated, objPosition.transform.position, objPosition.transform.rotation); 
                }
                Destroy(this.gameObject);
            }
        }
    }
}
