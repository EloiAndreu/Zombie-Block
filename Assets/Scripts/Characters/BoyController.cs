using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyController : MonoBehaviour
{
    [Header("Visió")]
    public float visionRange = 10f;
    [Range(0, 360)]
    public float visionAngle = 90f;
    public string playerTag = "Player"; 
    public string enemyTag = "Enemy"; 
    public LayerMask obstacleLayers;

    [Header("Spawn")]
    public GameObject boyScreamPrefab;
    bool hasSpawned = false;

    public float intervalTargets = 2f;
    public float intervalRecollibles = 5f;

    private float timerTargets = 0f;
    private float timerRecollibles = 0f;

    public LayerMask layerRecollible;
    public GameObject boyRobar;
    GameObject newBoyRobar = null;

    void Update()
    {
        timerTargets += Time.deltaTime;
        timerRecollibles += Time.deltaTime;

        if (timerTargets >= intervalTargets)
        {
            DetectTargets();
            timerTargets = 0f;
        }

        if (timerRecollibles >= intervalRecollibles)
        {
            DetectarObjectesRecollibles();
            timerRecollibles = 0f;
        }
    }

    void DetectarObjectesRecollibles()
    {
        //Debug.Log("Detectant objectes");
        Collider[] colls = Physics.OverlapSphere(transform.position, 50f, layerRecollible);

        List<GameObject> objectesValids = new List<GameObject>();

        foreach (Collider col in colls)
        {
            GameObject obj = col.gameObject;
            float y = obj.transform.position.y;

            if (y < 5f && y >= -0.5f)
            {
                objectesValids.Add(obj);
            }
        }

        if (objectesValids.Count > 0)
        {
            GameObject seleccionat = objectesValids[Random.Range(0, objectesValids.Count)];
            Debug.Log("Seleccionat: " + seleccionat.name);

            if(boyRobar != null && newBoyRobar == null)
            {
                newBoyRobar = Instantiate(boyRobar, transform.position, transform.rotation);

                //var script = newBoyRobar.GetComponent<CharacterRobarObj>();
                //script.Init(seleccionat, this.gameObject);

                newBoyRobar.GetComponent<CharacterRobarObj>().objecteInicial = seleccionat;
                newBoyRobar.GetComponent<CharacterRobarObj>().boyCompu = this.gameObject;
                
            }
        }
    }

    void OnEnabled()
    {
        newBoyRobar = null;
    }
    

    void DetectTargets()
    {
        // Ara detectem TOT (sense filtre de layer)
        Collider[] targetsInRange = Physics.OverlapSphere(transform.position, visionRange);

        foreach (Collider target in targetsInRange)
        {
            // 🔹 FILTRE PER TAG
            if (!target.CompareTag(playerTag) && !target.CompareTag(enemyTag))
                continue;

            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            if (angleToTarget < visionAngle / 2f)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                // 🔹 Comprovem obstacles
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleLayers))
                {
                    if (!hasSpawned)
                    {
                        hasSpawned = true;
                        //Instantiate(boyScreamPrefab, transform.position, transform.rotation);
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * visionRange);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * visionRange);
    }
}