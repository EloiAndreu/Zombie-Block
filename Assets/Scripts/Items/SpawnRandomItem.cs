using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomItem : MonoBehaviour
{
    public GameObject objectToSpawn;
    GameObject initialObject;
    public GameObject spawnPosition;
    ItemsManager itemsManager;
    bool hasSpawned = false;
    //public float spawnForce = 5f;
    GameObject objSpawned = null;


    void Start()
    {
        initialObject = objectToSpawn;
        itemsManager = GameObject.FindGameObjectWithTag("MainGame").GetComponent<ItemsManager>();
        if(objectToSpawn == null && itemsManager != null) objectToSpawn = itemsManager.GetRandomObject(transform.gameObject);
    }

    public void SpawnItem()
    {
        if(itemsManager == null || hasSpawned) return;
        hasSpawned = true;

        if(initialObject == null && !itemsManager.CheckItem(transform.gameObject)) return;

        //if(objectToSpawn == null) objectToSpawn = itemsManager.GetRandomObject();

        if(objectToSpawn != null && spawnPosition != null)
        {
            objSpawned = Instantiate(
                    objectToSpawn, 
                    spawnPosition.transform.position, 
                    spawnPosition.transform.rotation,
                    spawnPosition.transform
                );

            itemsManager.ObjectActive(transform.gameObject, false);
            /*Rigidbody rb = spawned.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddForce(Vector3.up * spawnForce, ForceMode.Impulse);
            }*/
        }
    }

    public void HideRandomItem()
    {
        if(objSpawned != null && objSpawned.transform.parent.gameObject == spawnPosition){
            hasSpawned = false;
            itemsManager.ObjectActive(transform.gameObject, true);
            Destroy(objSpawned);
        }
    }
}
