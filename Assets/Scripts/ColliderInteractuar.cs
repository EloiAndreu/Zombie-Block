using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderInteractuar : MonoBehaviour
{
    public UnityEvent[] interactEvents;
    public LayerMask layersToTrigger;

    void OnTriggerEnter(Collider coll)
    {
        if ((layersToTrigger.value & (1 << coll.gameObject.layer)) != 0)
        {
            Interactua();
        }
    }

    public void Interactua()
    {
        for (int i = 0; i < interactEvents.Length; i++)
        {
            interactEvents[i].Invoke();
        }
    }
}