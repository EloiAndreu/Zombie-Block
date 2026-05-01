using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyQuestion : MonoBehaviour
{
    GameObject player;
    public GameObject boyPatrol, boyLooting, boyFollowPlayer;
    public KeyCode followPlayerKey = KeyCode.Alpha4;
    public KeyCode lootingKey1 = KeyCode.Alpha1;
    public KeyCode lootingKey2 = KeyCode.Alpha2;
    public KeyCode lootingKey3 = KeyCode.Alpha3;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        transform.LookAt(new Vector3(player.transform.position.x, 0f, player.transform.position.z));

        if(Vector3.Distance(this.transform.position, player.transform.position) > 15f)
        {
            if(boyPatrol != null)
            {
                Instantiate(boyPatrol, transform.position, transform.rotation);
                Destroy(this.gameObject);
            }
        }
        else
        {
            MyInput();
        }
    }

    void MyInput(){ //Inputs
        if(Input.GetKeyDown (followPlayerKey)){
            if(boyFollowPlayer != null)
            {
                Instantiate(boyFollowPlayer, transform.position, transform.rotation);
                Destroy(this.gameObject);
            }
        }

        if(Input.GetKeyDown (lootingKey1)){
            if(boyLooting != null)
            {
                GameObject newboyLooting = Instantiate(boyLooting, transform.position, transform.rotation);
                newboyLooting.GetComponent<CharacterRobarObj>().SetCategoryToSearch(1);
                Destroy(this.gameObject);
            }
        }

        if(Input.GetKeyDown (lootingKey2)){
            if(boyLooting != null)
            {
                GameObject newboyLooting = Instantiate(boyLooting, transform.position, transform.rotation);
                newboyLooting.GetComponent<CharacterRobarObj>().SetCategoryToSearch(2);
                Destroy(this.gameObject);
            }
        }

        if(Input.GetKeyDown (lootingKey3)){
            if(boyLooting != null)
            {
                GameObject newboyLooting = Instantiate(boyLooting, transform.position, transform.rotation);
                newboyLooting.GetComponent<CharacterRobarObj>().SetCategoryToSearch(3);
                Destroy(this.gameObject);
            }
        }
    }
}
