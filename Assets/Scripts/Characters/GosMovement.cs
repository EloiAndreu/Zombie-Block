using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GosMovement : MonoBehaviour
{
    float originalSpeed, originalAcceleration, originalAngularSpeed, originalStoppingDist;
    private UnityEngine.AI.NavMeshAgent agent;
    private characterMoviment movement;
    
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        movement = GetComponent<characterMoviment>();

        originalSpeed = agent.speed;
        originalAcceleration = agent.acceleration;
        originalAngularSpeed = agent.angularSpeed;
        originalStoppingDist = agent.stoppingDistance;
    }


    public void PlayerTeOs()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 playerPos = player.transform.position;
        Vector3 gosPos = transform.position;

        // Direcció del gos cap al player
        Vector3 direccio = (playerPos - gosPos).normalized;

        // Punt 10 unitats abans d'arribar al player
        Vector3 target = playerPos - direccio * 10f;
        movement.OverrideMoveTo(target);

        agent.speed = 400f;
        agent.acceleration = 100f;
        agent.angularSpeed = 1200f;
        agent.stoppingDistance = 0f; // IMPORTANT

        StartCoroutine(WaitUntilArrives());
    }

    public void OsAlTerraLlencat()
    {
        GameObject osGos = GameObject.FindGameObjectWithTag("Os Gos");
        if (osGos == null) return;

        Vector3 target = osGos.transform.position;
        movement.OverrideMoveTo(target);

        agent.speed = 400f;
        agent.acceleration = 1000f;
        agent.angularSpeed = 1200f;
        agent.stoppingDistance = 0f; 

        StartCoroutine(WaitUntilArrives());
    }

    IEnumerator WaitUntilArrives()
    {
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // Esperem a que realment s’aturi
        while (agent.velocity.sqrMagnitude > 0.01f)
        {
            yield return null;
        }

        agent.isStopped = true;
    }

    public void PlayerDeixaOs()
    {
        agent.speed = originalSpeed;
        agent.acceleration = originalAcceleration;
        agent.angularSpeed = originalAngularSpeed;
        agent.stoppingDistance = originalStoppingDist;

        agent.isStopped = false;

        movement.StopOverride();
    }

    Vector3? GetRandomPointNearPlayer(float distance, int areaMask, int maxAttempts = 10)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return null;

        Vector3 origin = player.transform.position;

        for (int i = 0; i < maxAttempts; i++)
        {
            // Direcció aleatòria en pla (evitem Y)
            Vector2 randomCircle = Random.insideUnitCircle.normalized * distance;
            Vector3 randomPoint = origin + new Vector3(randomCircle.x, 0, randomCircle.y);

            Debug.DrawLine(origin, randomPoint, Color.red, 1f);

            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 5f, areaMask))
            {
                UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();

                if (UnityEngine.AI.NavMesh.CalculatePath(transform.position, hit.position, areaMask, path) &&
                    path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
                {
                    return hit.position;
                }
            }
        }

        return null;
    }
}
