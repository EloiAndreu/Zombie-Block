using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Instruccio1());
    }

    IEnumerator Instruccio1()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<Interactuable>().Interactua(0);
        yield return new WaitForSeconds(5f);
        GetComponent<Interactuable>().Interactua(1);
        StartCoroutine(Instruccio2());
    }

    IEnumerator Instruccio2()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<Interactuable>().Interactua(2);
        yield return new WaitForSeconds(10f);
        GetComponent<Interactuable>().Interactua(3);
        //StartCoroutine(Instruccio3());
    }

    IEnumerator Instruccio3()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<Interactuable>().Interactua(4);
        yield return new WaitForSeconds(10f);
        GetComponent<Interactuable>().Interactua(5);
        StartCoroutine(Instruccio4());
    }

    IEnumerator Instruccio4()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<Interactuable>().Interactua(6);
        yield return new WaitForSeconds(10f);
        GetComponent<Interactuable>().Interactua(7);
        //StartCoroutine(Instruccio3());
    }
}
