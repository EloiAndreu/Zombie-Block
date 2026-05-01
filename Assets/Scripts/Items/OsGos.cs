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
    Coroutine resetGos;

    public void LlencarOs()
    {
        if(resetGos != null)
        {
            StopCoroutine(resetGos);
        }
        resetGos = StartCoroutine(ResetGos());
        
        GameObject.FindGameObjectWithTag("Player")
            .GetComponent<Pickup>()
            .DropOs();

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        Vector3 direccio = Camera.main.transform.forward;

        osLlencat = true;
        rb.AddForce(direccio * throwForce, ForceMode.Impulse);
    }

    IEnumerator ResetGos()
    {
        float temps = 0f;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject gos = GameObject.FindGameObjectWithTag("Gos");

        Pickup pickup = player.GetComponent<Pickup>();

        while (temps < 5f)
        {
            // comprova constantment
            if (pickup.playerTeOs)
            {
                yield break; // surt de la coroutine immediatament
            }

            temps += Time.deltaTime;
            yield return null; // espera al següent frame
        }

        // si han passat els 5s i el player NO té os
        if (gos != null && !pickup.playerTeOs)
        {
            gos.GetComponent<GosMovement>().TornarAGosNormal();
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        //Debug.Log("Ha xocat " + osLlencat + ", " + ((capesValides & (1 << coll.gameObject.layer)) != 0));
        //if (!osLlencat) return;

        //Debug.Log(coll.gameObject.name);

        if (osLlencat && (capesValides & (1 << coll.gameObject.layer)) != 0)
        {
            if(resetGos != null)
            {
                StopCoroutine(resetGos);
            }

            osLlencat = false;
            esperarGos = true;
            osAlTerraEvent.Invoke();
            GameObject gosFollowPlayer = GameObject.FindGameObjectWithTag("Gos").gameObject;
            
            gosFollowPlayer.GetComponent<GosMovement>().OsAlTerraLlencat(this.gameObject);
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
