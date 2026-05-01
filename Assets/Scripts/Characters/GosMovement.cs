using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GosMovement : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;

    private GameObject player;
    private GameObject osActual;
    //bool potSeguirJugador = true;
    public GameObject gosNormal, gosAgressiu;

    private enum Estat
    {
        SeguintPlayer,
        AnantAOs
    }

    private Estat estatActual;

    [SerializeField] float distanciaMinimaPlayer = 10f;

    // Control de temps
    float timer = 0f;
    float interval = 3f;

    // Resultat de l'últim càlcul
    bool hiHaCami = true;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        estatActual = Estat.SeguintPlayer;
    }

    void Update()
    {
        transform.LookAt(new Vector3(player.transform.position.x, 0f, player.transform.position.z));
        timer += Time.deltaTime;

        // Només recalculam camí cada X segons
        if (timer >= interval)
        {
            timer = 0f;
            //potSeguirJugador = true;
            RecalcularCami();
        }

        if (estatActual == Estat.SeguintPlayer)
        {
            MouSeguintPlayer();
        }
        else if (estatActual == Estat.AnantAOs)
        {
            MouCapAOs();
        }
    }

    void RecalcularCami()
    {
        if (estatActual == Estat.SeguintPlayer && player != null)
        {
            hiHaCami = HiHaCami(player.transform.position);
        }
        else if (estatActual == Estat.AnantAOs && osActual != null)
        {
            hiHaCami = HiHaCami(osActual.transform.position);

            // Si no hi ha camí cap a l’os → destruir
            if (!hiHaCami)
            {
                Instantiate(gosNormal, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }

    void MouSeguintPlayer()
    {
        if (player == null) return;

        Vector3 dir = (transform.position - player.transform.position).normalized;
        Vector3 target = player.transform.position + dir * distanciaMinimaPlayer;

        // Sempre comprovem cap al mateix punt
        //hiHaCami = HiHaCami(target);

        if (hiHaCami)
        {
            agent.isStopped = false;
            agent.SetDestination(target);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    void MouCapAOs()
    {
        if (osActual == null) return;

        if (!hiHaCami) return;

        agent.isStopped = false;
        //Debug.Log("Seguint Os");
        agent.SetDestination(osActual.transform.position);

        // Comprovació d'arribada
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (agent.velocity.sqrMagnitude < 0.01f)
            {
                Instantiate(gosAgressiu, transform.position, transform.rotation);
                Destroy(osActual.gameObject);
                Destroy(gameObject);
            }
        }
    }

    bool HiHaCami(Vector3 desti)
    {
        UnityEngine.AI.NavMeshHit hit;

        // Ajustem el destí al NavMesh
        if (!UnityEngine.AI.NavMesh.SamplePosition(desti, out hit, 10f, UnityEngine.AI.NavMesh.AllAreas)){
            //Debug.Log("Hi ha cami: false 1");
            return false;
        }

        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();

        if (agent.CalculatePath(hit.position, path))
        {
            //bool hiHaCami = (path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete);
            bool hiHaCami = 
                path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete ||
                path.status == UnityEngine.AI.NavMeshPathStatus.PathPartial;
            //Debug.Log("Hi ha cami: " + hiHaCami + " 2");
            return path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete;
        }

        //Debug.Log("Hi ha cami: false 3");
        return false;
    }

    public void OsAlTerraLlencat(GameObject osGos)
    {
        if (osGos == null) return;

        osActual = osGos;
        estatActual = Estat.AnantAOs;

        // Forcem càlcul immediat (no esperar 3s)
        hiHaCami = HiHaCami(osActual.transform.position);

        if (!hiHaCami)
        {
            Instantiate(gosNormal, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    public void TornarAGosNormal()
    {
        Instantiate(gosNormal, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}