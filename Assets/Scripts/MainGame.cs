using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.AI;

[Serializable]
public class Ronda{
    public List<GrupEnemics> grupEnemics;
    public float tempsSeguentRonda;
}

[Serializable]
public class GrupEnemics{
    public GameObject prefabEnemy;
    public int quantitat;
}

public class MainGame : MonoBehaviour
{
    public int dificultatJoc = 1; //0 - fàcil, 1 - normal, 2 - difícil

    public float minTempsEntreEnemics, maxTempsEntreEnemics;
    public float tempsPreparacio; //Temps abans de començar la primera ronda

    public List<Ronda> rondes;
    public int rondaActual = -1;

    public BoxCollider[] areesDeSpawn;
    public Transform  zombiParent;

    Coroutine preparacioJugador, esperarSeguentRonda; 

    public TMP_Text ronda_text, ronda_text_2;

    public UnityEvent començarRondaEvent, rondaComençadaEvent;

    void Start()
    {
        preparacioJugador = StartCoroutine(PreparacioJugador());
    }

    IEnumerator PreparacioJugador()
    {
        yield return new WaitForSeconds(tempsPreparacio);
        IniciarRonda();
    }

    public void IniciarRonda()
    {
        if(preparacioJugador != null) StopCoroutine(preparacioJugador);
        if(esperarSeguentRonda != null ) StopCoroutine(esperarSeguentRonda);

        rondaActual++;
        if(rondaActual < rondes.Count - 1)
        {
            ronda_text.text = "RONDA " + (rondaActual+1);
            ronda_text_2.text = "RONDA " + (rondaActual+1);
            StartCoroutine(GenerarEnemics(rondaActual));
        }
    }

    IEnumerator GenerarEnemics(int rondaID)
    {   
        //ronda_text.text = "RONDA " + (rondaID+1); // + "/" + rondes.Count;
        començarRondaEvent.Invoke();

        yield return new WaitForSeconds(3f);
        rondaComençadaEvent.Invoke();

        for(int i=0; i<rondes[rondaID].grupEnemics.Count; i++){
            int nEnemics = 0;
            while (nEnemics < rondes[rondaID].grupEnemics[i].quantitat)
            {
                GenerarEnemic(rondes[rondaID].grupEnemics[i].prefabEnemy);
                nEnemics++;

                float tempsDeEspera = UnityEngine.Random.Range(minTempsEntreEnemics, maxTempsEntreEnemics);
                yield return new WaitForSeconds(tempsDeEspera);
            }
        }

        esperarSeguentRonda = StartCoroutine(EsperarSeguentRonda(rondaID));
    }

    IEnumerator EsperarSeguentRonda(int rondaID)
    {
        yield return new WaitForSeconds(rondes[rondaID].tempsSeguentRonda);
        IniciarRonda();
    }

    void GenerarEnemic(GameObject prefabEnemy)
    {
        int areaSpawn = UnityEngine.Random.Range(0, areesDeSpawn.Length);
        Bounds bounds = areesDeSpawn[areaSpawn].bounds;

        float randomX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);

        Vector3 spawnPos = new Vector3(randomX, 0f, randomZ);

        GameObject z = Instantiate(prefabEnemy, spawnPos, Quaternion.identity, zombiParent);
        NavMeshAgent agent = z.GetComponent<NavMeshAgent>();

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); // ✅ reposiciona l'agent correctament al NavMesh
        }
    }

    public void ValidarEnemicsVius()
    {
        GameObject[] enemics = GameObject.FindGameObjectsWithTag("Enemy");

        if(enemics.Length == 0)
        {
            IniciarRonda();
        }
    }
}
