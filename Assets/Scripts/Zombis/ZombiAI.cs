using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class ZombieAI : MonoBehaviour
{
    public float detectionRadius = 50f;
    public float attackDistance = 2f;
    public float updateTargetRate = 0.5f;
    public float minDamage = 20f, maxDamage = 50f;
    public float distAvoidObject = 30f;

    private NavMeshAgent agent;
    private NavMeshPath path;
    private Transform currentTarget;

    private int playerLayer, familiarLayer, destructibleLayer;
    public MultiAimConstraint aimConstraint;
    public Animator anim;
    

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();

        playerLayer = LayerMask.NameToLayer("Player");
        familiarLayer = LayerMask.NameToLayer("Family Member");
        destructibleLayer = LayerMask.NameToLayer("Destructible");

        InvokeRepeating(nameof(FindClosestHumanInvoker), 0f, updateTargetRate);
    }

    void UpdateAimTarget(Transform target)
    {
        if (aimConstraint == null || target == null) return;

        var data = aimConstraint.data;

        WeightedTransformArray sources = data.sourceObjects;
        sources.Clear();
        sources.Add(new WeightedTransform(target, 1f));

        data.sourceObjects = sources;
        aimConstraint.data = data;
    }

    float GetPathLength(NavMeshPath p)
    {
        float length = 0f;
        for (int i = 1; i < p.corners.Length; i++)
        {
            length += Vector3.Distance(p.corners[i - 1], p.corners[i]);
        }
        return length;
    }

    void Update()
    {
        if (currentTarget == null) return;
        Vector3 center = currentTarget.GetComponent<BoxCollider>().bounds.center;
        center = new Vector3(center.x, 0f, center.z);

        float distance = Vector3.Distance(transform.position, center);

        if (distance <= attackDistance)
        {
            if(agent.enabled == true) agent.isStopped = true;
            anim.SetTrigger("Attack");
        }
        else
        {
            if(agent.enabled == true) {
                int layer = currentTarget.gameObject.layer;

                if (layer == destructibleLayer)
                {
                    // Si és destructible ignorem validació del path
                    agent.isStopped = false;
                    agent.SetDestination(center);
                }
                else
                {
                    if (agent.CalculatePath(center, path)) // Per defecte intentem sempre 
                    {
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            float directDistance = Vector3.Distance(transform.position, center);
                            float pathDistance = GetPathLength(path);

                            if(pathDistance < directDistance + distAvoidObject)
                            {
                                agent.isStopped = false;
                                agent.SetDestination(center);
                            }
                            else
                            {
                                FindClosestHuman(false);
                            }

                        }
                        else
                        {
                            FindClosestHuman(false);
                        }
                    }
                }
            }
        }
    }

    void FindClosestHumanInvoker()
    {
        FindClosestHuman(true);
    }

    void FindClosestHuman(bool character)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider col in hits)
        {
            int layer = col.gameObject.layer;

            if (((layer == playerLayer || layer == familiarLayer) && character) || (layer == destructibleLayer && !character))
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);

                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = col.transform;
                }
            }
        }

        currentTarget = closest;
        UpdateAimTarget(Camera.main.transform);
    }

    public void Atac(){
        if(currentTarget != null && currentTarget.GetComponent<Health>() != null){
            float damage = Random.Range(minDamage, maxDamage);
            Rigidbody rbTarget = currentTarget.GetComponent<Rigidbody>();
            bool deadEnemy = currentTarget.GetComponent<Health>().TakeDamage(damage, rbTarget, Vector3.zero);
            if(deadEnemy){
                FindClosestHumanInvoker();
            }
        }

        if(currentTarget == null){
            FindClosestHumanInvoker();
        }
    }
}