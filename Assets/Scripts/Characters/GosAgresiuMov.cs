using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GosAgresiuMov : MonoBehaviour
{
    public float updateTargetRate = 0.5f;
    public float detectionRadius = 50f;
    public float attackDistance = 2f;
    public float minDamage = 20f, maxDamage = 50f;
    public float distAvoidObject = 30f;
    public float tempsAgresiu = 10f;

    private UnityEngine.AI.NavMeshAgent agent;
    private UnityEngine.AI.NavMeshPath path;
    private Transform currentTarget;

    public MultiAimConstraint aimConstraint;
    public Animator anim;

    public bool atacking = false;
    public GameObject particlesBlood;
    public GameObject particleSpawnPoint;
    public GameObject gosNormal;
    Coroutine atacCoroutine;
    

    void Start()
    {
        StartCoroutine(CompteEnrrereAgressiu());
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        path = new UnityEngine.AI.NavMeshPath();

        InvokeRepeating(nameof(FindClosestEnemyInvoker), 0f, updateTargetRate);
    }

    IEnumerator CompteEnrrereAgressiu()
    {
        yield return new WaitForSeconds(tempsAgresiu);
        Instantiate(gosNormal, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
    
    void FindClosestEnemyInvoker()
    {
        FindClosestEnemy();
    }

    GameObject FindAncestorWithTag(Transform t, string tag)
    {
        while (t != null)
        {
            if (t.CompareTag(tag))
                return t.gameObject;
            t = t.parent;
        }
        return null;
    }

    void FindClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        HashSet<Transform> processedEnemies = new HashSet<Transform>();

        foreach (Collider col in hits)
        {
            GameObject enemyNode = FindAncestorWithTag(col.transform, "Enemy");

            if (enemyNode != null)
            {
                Transform enemyTransform = enemyNode.transform;

                if (processedEnemies.Contains(enemyTransform))
                    continue;

                processedEnemies.Add(enemyTransform);

                Health health = enemyNode.GetComponent<Health>();

                if (health != null && health.currentHealth > 0)
                {
                    float dist = Vector3.Distance(transform.position, enemyTransform.position);

                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closest = enemyTransform;
                    }
                }
            }
        }

        currentTarget = closest;
    }

    void UpdateAimTarget(Transform target)
    {
        if (aimConstraint == null || target == null) return;

        var data = aimConstraint.data;

        data.sourceObjects.Clear();
        data.sourceObjects.Add(new WeightedTransform(target, 1f));

        aimConstraint.data = data;
    }

    void Update()
    {
        if (currentTarget == null) return;
        Vector3 center = currentTarget.transform.position;

        float distance = Vector3.Distance(transform.position, center);

        if (!atacking && distance <= attackDistance)
        {
            atacking = true;
            if(agent.enabled == true) agent.isStopped = true;
            if(anim != null) anim.SetTrigger("Attack");
            //Debug.Log("AttacK");
            atacCoroutine = StartCoroutine(Atacking());
        }
        else if(!atacking)
        {
            if(agent.enabled == true) {
                int layer = currentTarget.gameObject.layer;

                if (agent.CalculatePath(center, path)) // Per defecte intentem sempre 
                {
                    if (path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
                    {
                        agent.isStopped = false;
                        agent.SetDestination(center);

                    }
                    else FindClosestEnemy();
                }
                
            }
        }
    }

    IEnumerator Atacking()
    {
        while(currentTarget != null)
        {
            Health health = currentTarget.GetComponent<Health>();

            if (health == null || health.currentHealth <= 0)
                break;

            Atac();
            yield return new WaitForSeconds(0.5f);
        }

        atacking = false;
    }

    public void Atac(){
        Health health = currentTarget.GetComponent<Health>();
        if(currentTarget != null && health != null){
            float damage = Random.Range(minDamage, maxDamage);
            Rigidbody rbTarget = currentTarget.GetComponent<Rigidbody>();
            bool deadEnemy = health.TakeDamage(damage, rbTarget, Vector3.zero);
            
            if (particlesBlood != null)
            {
                Instantiate(particlesBlood, particleSpawnPoint.transform.position, particleSpawnPoint.transform.rotation);
            }

            if(deadEnemy){
                if (atacCoroutine != null) StopCoroutine(atacCoroutine);
                atacking = false;
                if(agent.enabled == true) agent.isStopped = false;
                FindClosestEnemyInvoker();
            }
        }

        if(currentTarget == null){
            if (atacCoroutine != null) StopCoroutine(atacCoroutine);
            atacking = false;
            if(agent.enabled == true) agent.isStopped = false;
            FindClosestEnemyInvoker();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
