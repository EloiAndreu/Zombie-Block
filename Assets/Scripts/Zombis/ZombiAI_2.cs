using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class ZombieAI_2 : MonoBehaviour
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

    Vector3 lastDestination;
    //float humanPriorityMultiplier = 0.7f; 
    
    int ballesDestruides = 0;

    public bool atacking = false;
    Coroutine resetCoroutine;

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
        Vector3 center = currentTarget.GetComponent<Collider>().ClosestPoint(transform.position);
        center = new Vector3(center.x, 1f, center.z);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(center, out hit, 2.0f, NavMesh.AllAreas))
        {
            center = hit.position;
        }

        float distance = Vector3.Distance(transform.position, center);

        if (distance <= attackDistance)
        {
            if(agent.enabled == true) agent.isStopped = true;

            //Debug.Log(currentTarget.gameObject.name);

            if (!atacking)
            {
                atacking = true;

                if (resetCoroutine != null)
                    StopCoroutine(resetCoroutine);

                resetCoroutine = StartCoroutine(ResetAtac());

                anim.SetTrigger("Attack");
            }
        }
        else
        {
            //agent.isStopped = false;

            if(agent.enabled == true) {
                agent.isStopped = false;
                int layer = currentTarget.gameObject.layer;

                if (Vector3.Distance(lastDestination, center) > 1f){

                    if (layer == destructibleLayer)
                    {
                        // Si és destructible ignorem validació del path
                        agent.isStopped = false;
                        //Debug.Log("SET DESTINATION 1 - Destructible: "+ currentTarget.gameObject.name);
                        agent.SetDestination(center);
                        lastDestination = center;
                    }
                    else
                    {
                        if (agent.CalculatePath(center, path)) // Per defecte intentem sempre 
                        {
                            if (path.status == NavMeshPathStatus.PathComplete)
                            {
                                agent.isStopped = false;
                                //Debug.Log("SET DESTINATION 2: "+ currentTarget.gameObject.name);
                                agent.SetDestination(center);
                                lastDestination = center;

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
    }

    IEnumerator ResetAtac()
    {
        yield return new WaitForSeconds(3f);
        atacking = false;
    }

    void FindClosestHumanInvoker()
    {
        FindClosestHuman(true);
    }

    void FindClosestHuman(bool character)
    {
        if(atacking) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        Transform closestReachableHuman = null;
        float closestHumanDist = Mathf.Infinity;

        Transform closestFallback = null;
        float closestFallbackDist = Mathf.Infinity;

        foreach (Collider col in hits)
        {
            int layer = col.gameObject.layer;
            float dist = Vector3.Distance(transform.position, col.transform.position);

            // 🔵 PRIORITAT: Player / Familiar
            if (layer == playerLayer || layer == familiarLayer)
            {
                Vector3 targetPos = col.GetComponent<Collider>().ClosestPoint(transform.position);
                targetPos = new Vector3(targetPos.x, 1f, targetPos.z);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(targetPos, out hit, 2.0f, NavMesh.AllAreas))
                {
                    targetPos = hit.position;
                }

                if (IsReachable(targetPos))
                {
                    if (dist < closestHumanDist)
                    {
                        closestHumanDist = dist;
                        closestReachableHuman = col.transform;
                    }
                }
            }

            // 🟡 FALLBACK: destructibles
            if ((layer == destructibleLayer && ballesDestruides == 0) ||
                (col.CompareTag("Interactuable") && layer == destructibleLayer && ballesDestruides > 0))
            {
                if (dist < closestFallbackDist)
                {
                    closestFallbackDist = dist;
                    closestFallback = col.transform;
                }
            }
        }

        // 🎯 DECISIÓ FINAL
        if (closestReachableHuman != null)
        {
            if(closestHumanDist < closestFallbackDist + 30)
            {
                currentTarget = closestReachableHuman;
                //Debug.Log("Target HUMÀ reachable");
            }
            else
            {
                currentTarget = closestFallback;
                //Debug.Log("Fallback DESTRUCTIBLE");
            }
        }
        else
        {
            currentTarget = closestFallback;
            //Debug.Log("Fallback DESTRUCTIBLE");
        }

        UpdateAimTarget(Camera.main.transform);
    }

    bool IsReachable(Vector3 targetPos)
    {
        if (!agent.isOnNavMesh) return false;

        if (agent.CalculatePath(targetPos, path))
        {
            return path.status == NavMeshPathStatus.PathComplete;
        }

        return false;
    }

    public void Atac(){
        
        if(currentTarget != null && currentTarget.GetComponent<Health>() != null){
            //Debug.Log("Attac");
            
            float damage = Random.Range(minDamage, maxDamage);
            Rigidbody rbTarget = currentTarget.GetComponent<Rigidbody>();

            bool deadEnemy = currentTarget.GetComponent<Health>().TakeDamage(damage, rbTarget, Vector3.zero);
            if(deadEnemy){
                if(currentTarget.gameObject.layer == destructibleLayer)
                {
                    ballesDestruides++;
                }

                FindClosestHumanInvoker();
            }
        }

        if(currentTarget == null){
            FindClosestHumanInvoker();
        }
        
        atacking = false;
    }
}