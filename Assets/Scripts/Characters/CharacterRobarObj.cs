using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CharacterRobarObj : MonoBehaviour
{
    private NavMeshAgent agent;

    public GameObject objecteInicial;
    public GameObject boyCompu, boyStress, boyPatrol, boyGiveItem, boyNoPot;

    private Transform target;
    private Transform targetOriginal;

    private Vector3 posicioInicial;
    GameObject player;

    enum Estat { AnarObjecte, TornarInici, AnarPorta, EsperantPorta }
    Estat estatActual;

    float timer = 0f;
    float interval = 3f;

    //public GameObject boyMesh;
    public bool robar = true;

    ItemsManager itemsManager;
    ItemsManager.ItemSpawned itemSpawned;

    //private GameObject portaExterior;
    //public GameObject portaInterior;
    //GameObject[] portes;

    void Awake()
    {
        itemsManager = GameObject.FindGameObjectWithTag("MainGame").GetComponent<ItemsManager>();
        /*portes = GameObject.FindGameObjectsWithTag("Porta Exterior");

        if (portes.Length > 0)
        {
            portaExterior = portes[Random.Range(0, portes.Length)];
        }*/
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (robar)
        {
            //boyMesh.SetActive(false);
            posicioInicial = transform.position;

            if (objecteInicial != null)
            {
                target = objecteInicial.transform;
                estatActual = Estat.AnarObjecte;
                RecalcularCami();
            }
        }
    }

    public void SetCategoryToSearch(int category)
    {
        itemSpawned = itemsManager.getItemToSearch(category);

        if (itemSpawned == null)
        {
            //Debug.LogWarning("No s'ha trobat cap item per la categoria");
            if (boyNoPot != null){
                GameObject newBoy = Instantiate(boyNoPot, transform.position, transform.rotation);
                //newBoy.GetComponent<BoyGiveObject>().SetObjectHand(itemSpawned.item);
            }
            return;
        }

        target = itemSpawned.parent.transform;
        estatActual = Estat.AnarObjecte;
        RecalcularCami();
    }

    void Update()
    {
        if (target == null) return;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            RecalcularCami();
        }

        Mou();
    }

    void RecalcularCami()
    {
        if (!robar)
            posicioInicial = player.transform.position;

        Vector3 desti = (estatActual == Estat.AnarObjecte) ? target.position : posicioInicial;

        if (!HiHaCami(desti))
        {
            if (estatActual == Estat.AnarObjecte)
            {
                /*Debug.Log("No hi ha camí → intentem porta");

                Transform porta = GetPortaAccessible();

                if (porta != null)
                {
                    targetOriginal = target;
                    target = porta;
                    estatActual = Estat.AnarPorta;
                    return;
                }

                Debug.Log("No hi ha cap porta accessible → tornem inici");*/
                //Debug.Log("No hi ha camí → tornem inici");
                estatActual = Estat.TornarInici;
            }
            else if (estatActual == Estat.TornarInici)
            {
                /*Debug.Log("No hi ha camí de tornada → intentem porta");

                Transform porta = GetPortaAccessible();

                if (porta != null)
                {
                    targetOriginal = target;
                    target = porta;
                    estatActual = Estat.AnarPorta;
                    return;
                }

                Debug.Log("Sense porta accessible → destruir");*/

                //Debug.Log("No hi ha camí de tornada → destruir");

                if (boyStress != null)
                    Instantiate(boyStress, transform.position, transform.rotation);

                Destroy(gameObject);
            }
        }
        /*else
        {
            boyMesh.SetActive(true);
            boyCompu.SetActive(false);
        }*/
    }

    /*Transform GetPortaAccessible()
    {
        portaExterior = portes[Random.Range(0, portes.Length)];
        if (portaExterior != null && HiHaCami(portaExterior.transform.position))
            return portaExterior.transform;

        //if (portaInterior != null && HiHaCami(portaInterior.transform.position))
            //return portaInterior.transform;

        return null;
    }*/

    void Mou()
    {
        if (!agent.isOnNavMesh || !agent.enabled) return;

        if (!robar)
            posicioInicial = player.transform.position;

        Vector3 desti;

        if (estatActual == Estat.AnarObjecte) // || estatActual == Estat.AnarPorta)
            desti = target.position;
        else
            desti = posicioInicial;

        agent.SetDestination(desti);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (agent.velocity.sqrMagnitude < 0.01f)
            {
                if (estatActual == Estat.AnarObjecte)
                {
                    //Debug.Log("Objecte agafat → tornem al inici");
                    estatActual = Estat.TornarInici;
                }
                /*else if (estatActual == Estat.AnarPorta)
                {
                    Debug.Log("Arribat a porta → interactuem");
                    StartCoroutine(InteractuarAmbPorta());
                }*/
                else if (estatActual == Estat.TornarInici)
                {
                    //Debug.Log("Arribat a origen → destroy");
                    //boyCompu.SetActive(true);
                    if (boyGiveItem != null){
                        GameObject newBoy = Instantiate(boyGiveItem, transform.position, transform.rotation);
                        newBoy.GetComponent<BoyGiveObject>().SetObjectHand(itemSpawned.item);
                    }
                        //Instantiate(boyPatrol, transform.position, transform.rotation);

                    Destroy(gameObject);
                }
            }
        }
    }

    /*IEnumerator InteractuarAmbPorta()
    {
        estatActual = Estat.EsperantPorta;

        agent.isStopped = true;
        portaExterior.transform.GetChild(0).transform.GetComponent<Interactuable>().Interactua(0);

        Debug.Log("Esperant 2 segons a la porta...");
        yield return new WaitForSeconds(2f);

        agent.isStopped = false;

        // Tornem a intentar anar a l'objecte original
        target = targetOriginal;
        estatActual = Estat.AnarObjecte;

        Debug.Log("Reintentant camí cap a objecte");
    }*/

    bool HiHaCami(Vector3 desti)
    {
        NavMeshHit hit;

        if (!NavMesh.SamplePosition(desti, out hit, 10f, NavMesh.AllAreas))
            return false;

        NavMeshPath path = new NavMeshPath();

        if (agent.CalculatePath(hit.position, path))
        {
            return path.status == NavMeshPathStatus.PathComplete;
        }

        return false;
    }
}