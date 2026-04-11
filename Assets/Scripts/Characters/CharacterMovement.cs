using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class characterMoviment : MonoBehaviour
{
    public enum MovementType
    {
        Waypoints,
        RandomNavMesh
    }

    [Header("General")]
    public MovementType movementType;
    public float waitTime = 2f;

    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Random NavMesh")]
    public float randomRadius = 10f;
    public int maxAttempts = 10;
    public int areaMask = NavMesh.AllAreas;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float waitCounter;
    private bool waiting = false;

    private bool overrideMovement = false;
    //private bool hasStartedMoving = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (movementType == MovementType.Waypoints && waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
        else if (movementType == MovementType.RandomNavMesh)
        {
            SetRandomDestination();
        }
    }

    void Update()
    {
        if (!agent.enabled) return;

        if (overrideMovement) return;

        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            //hasStartedMoving = true;
        }


        if (!waiting 
            && !agent.pathPending 
            && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            StartWaiting();
        }
        

        if (waiting)
        {
            waitCounter += Time.deltaTime;

            if (waitCounter >= waitTime)
            {
                waiting = false;
                agent.isStopped = false;

                if (movementType == MovementType.Waypoints)
                {
                    GoToNextWaypoint();
                }
                else if (movementType == MovementType.RandomNavMesh)
                {
                    SetRandomDestination();
                }
            }
        }
    }

    void StartWaiting()
    {
        waiting = true;
        waitCounter = 0f;
        agent.isStopped = true;
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        agent.SetDestination(waypoints[currentWaypointIndex].position);

        //hasStartedMoving = false;
    }

    void SetRandomDestination()
    {
        Vector3 randomPoint;
        if (GetRandomPointOnNavMesh(transform.position, randomRadius, out randomPoint))
        {
            agent.SetDestination(randomPoint);
            //hasStartedMoving = false; // encara no s’ha mogut
        }
    }

    IEnumerator TrySetRandomDestination()
    {
        yield return new WaitForSeconds(0.1f);
        SetRandomDestination();
    }

    bool GetRandomPointOnNavMesh(Vector3 origin, float radius, out Vector3 result)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius + origin;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 2.0f, areaMask))
            {
                NavMeshPath path = new NavMeshPath();

                if (agent.CalculatePath(hit.position, path) 
                    && path.status == NavMeshPathStatus.PathComplete)
                {
                    result = hit.position;
                    return true;
                }
            }
        }

        // fallback segur
        NavMeshHit fallbackHit;
        if (NavMesh.SamplePosition(origin, out fallbackHit, radius, areaMask))
        {
            result = fallbackHit.position;
            return true;
        }

        result = origin;
        return false;
    }

    public void SetTarget(Vector3 target)
    {
        agent.SetDestination(target);
    }

    public void OverrideMoveTo(Vector3 target)
    {
        overrideMovement = true;
        waiting = false;
        agent.isStopped = false;
        agent.SetDestination(target);
    }

    public void StopOverride()
    {
        overrideMovement = false;
        waiting = false;
        agent.isStopped = false;

        // Tornar al comportament normal
        if (movementType == MovementType.Waypoints)
        {
            GoToNextWaypoint();
        }
        else if (movementType == MovementType.RandomNavMesh)
        {
            SetRandomDestination();
        }
    }
}