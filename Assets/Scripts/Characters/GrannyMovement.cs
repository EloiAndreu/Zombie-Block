using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrannyMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public float waitTime = 2f;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float waitCounter;
    private bool waiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        if (agent.enabled && !waiting && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartWaiting();
        }

        if (waiting)
        {
            waitCounter += Time.deltaTime;

            if (waitCounter >= waitTime)
            {
                GoToNextWaypoint();
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
        if(agent.enabled){
            waiting = false;
            agent.isStopped = false;

            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}