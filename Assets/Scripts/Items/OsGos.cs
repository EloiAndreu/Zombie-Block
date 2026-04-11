using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OsGos : MonoBehaviour
{
    public float throwForce = 50f;
    private Rigidbody rb;
    bool osLlencat = false;
    bool esperarGos = false;
    public LayerMask capesValides;
    public UnityEvent osAlTerraEvent;

    public GameObject GosAgresiuPrefab;

    public void LlencarOs()
    {
        GameObject.FindGameObjectWithTag("Player")
            .GetComponent<Pickup>()
            .DropObject();

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        Vector3 direccio = Camera.main.transform.forward;

        osLlencat = true;
        rb.AddForce(direccio * throwForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision coll)
    {
        //if (!osLlencat) return;

        //Debug.Log(coll.gameObject.name);

        if (osLlencat && (capesValides & (1 << coll.gameObject.layer)) != 0)
        {
            osLlencat = false;
            esperarGos = true;
            osAlTerraEvent.Invoke();
        }

        if(esperarGos && coll.gameObject.tag == "Gos")
        {
            esperarGos = false;
            Instantiate(GosAgresiuPrefab, coll.gameObject.transform.position, coll.gameObject.transform.rotation);
            Destroy(coll.gameObject);
            Destroy(this.gameObject);
        }

    }
}
