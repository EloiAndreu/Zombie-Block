using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public List<GameObject> elementsInstr1, elementsInstr2, elementsInstr3, elementsInstr4;

    void Start()
    {
        StartCoroutine(Instruccio1());
    }

    void HabilitarElements(bool enabled, List<GameObject> elements)
    {
        for(int i=0; i<elements.Count; i++)
        {
            if(enabled) elements[i].GetComponent<Animator>().SetTrigger("Enfosquir");
            else elements[i].GetComponent<Animator>().SetTrigger("Aclarir");
        }
    }

    IEnumerator Instruccio1()
    {
        yield return new WaitForSeconds(2f);
        HabilitarElements(true, elementsInstr1);
        yield return new WaitForSeconds(5f);
        HabilitarElements(false, elementsInstr1);
        StartCoroutine(Instruccio2());
    }

    IEnumerator Instruccio2()
    {
        yield return new WaitForSeconds(2f);
        HabilitarElements(true, elementsInstr2);
        yield return new WaitForSeconds(10f);
        HabilitarElements(false, elementsInstr2);
        StartCoroutine(Instruccio3());
    }

    IEnumerator Instruccio3()
    {
        yield return new WaitForSeconds(2f);
        HabilitarElements(true, elementsInstr3);
        yield return new WaitForSeconds(10f);
        HabilitarElements(false, elementsInstr3);
        StartCoroutine(Instruccio4());
    }

    IEnumerator Instruccio4()
    {
        yield return new WaitForSeconds(2f);
        HabilitarElements(true, elementsInstr4);
        yield return new WaitForSeconds(10f);
        HabilitarElements(false, elementsInstr4);
        //StartCoroutine(Instruccio3());
    }
}
