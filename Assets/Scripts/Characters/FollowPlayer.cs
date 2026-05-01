using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private GameObject player;
    [SerializeField] float distanciaMinimaPlayer = 10f;
    public GameObject boyPatrol;
    bool hiHaCami = true;

    // Control de temps
    float timer = 0f;
    float interval = 3f;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        transform.LookAt(new Vector3(player.transform.position.x, 0f, player.transform.position.z));
        timer += Time.deltaTime;

        // Només recalculam camí cada X segons
        if (timer >= interval)
        {
            timer = 0f;
            RecalcularCami();
        }

        MouSeguintPlayer();
    }

    void RecalcularCami()
    {
        hiHaCami = HiHaCami(player.transform.position);
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
            if(boyPatrol != null)
            {
                Instantiate(boyPatrol, transform.position, transform.rotation);
                Destroy(this.gameObject);
            }
        }
    }
}
