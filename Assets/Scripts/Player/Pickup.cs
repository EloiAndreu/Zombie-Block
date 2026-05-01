using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//COMPORTAMENT DEL JUGADOR PER AGAFAR OBJECTES
public class Pickup : MonoBehaviour
{
    [Header("Propietats")]
    public float pickupMaxDist = 5f;
    bool hasObject = false;

    [Header("Elements externs")]
    public GameObject gunPosition;
    public GameObject obj;
    public GameObject objToStart;

    [Header("Inputs")]
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;

    [Header("Arrossegar")]
    bool arrossegaObject = false;
    public float dragDistance = 3f;
    public float moveSpeed = 10f;

    private Transform draggedObject;
    private Rigidbody draggedRb;
    private Vector3 offset;
    GameObject objOriginal;
    GameObject objAMoure;

    public LayerMask pickupLayer;
    public GameObject potInteractuarImg;
    public GameObject gosFollowPlayer;
    public bool playerTeOs = false;

    void Start()
    {
        if(objToStart != null && objToStart.layer == 15)
        {
            hasObject = true;

            obj = objToStart;
            obj.transform.parent = gunPosition.transform;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            obj.GetComponent<BoxCollider>().enabled = false;

            obj.transform.position = new Vector3(0f, 0f, 0f);
            obj.transform.localPosition = new Vector3(0f, 0f, 0f);
            obj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            if(obj.GetComponent<Animator>() != null) obj.GetComponent<Animator>().enabled = true;
        }
    }

    void Update()
    {
        MyInput();
        CheckInteractua();
    }

    void CheckInteractua()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupMaxDist, pickupLayer))
        {
            if (hit.collider.CompareTag("Interactuable") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Recollible") || 
                ((hit.collider.gameObject.layer == LayerMask.NameToLayer("Family Member")) && hit.collider.transform.GetComponent<Interactuable>() != null))
            {
                potInteractuarImg.SetActive(true);
                return;
            }
        }

        potInteractuarImg.SetActive(false);
    }


    void MyInput(){ //Inputs
        if(Input.GetKeyDown (pickupKey) && !arrossegaObject){
            PickUpObject(); //Agafar objecte
        }

        if(Input.GetKeyDown (dropKey) && hasObject){
            DropObject(); //Deixar objecte
        }

        if (Input.GetMouseButtonDown(0) && draggedRb == null && !hasObject)
        {
            TryStartDrag();
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            StopDrag();
        }
    }

    void PickUpObject(){
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupMaxDist, pickupLayer)){
            //Si té el tag "Object" el col·loquem a la mà
            if (hit.collider.gameObject.layer == 15 && !hasObject){
                hasObject = true;

                obj = hit.collider.gameObject;
                obj.transform.parent = gunPosition.transform;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
                obj.GetComponent<BoxCollider>().enabled = false;

                obj.transform.position = new Vector3(0f, 0f, 0f);
                obj.transform.localPosition = new Vector3(0f, 0f, 0f);
                obj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                MostrarUIObjecte(obj, true);

                if(obj.GetComponent<Animator>() != null) obj.GetComponent<Animator>().enabled = true;

                if(obj.transform.tag == "Os Gos")
                {
                    playerTeOs = true;
                    GameObject gosActual = GameObject.FindGameObjectWithTag("Gos").gameObject;
                    if(gosActual != null){
                        Instantiate(gosFollowPlayer, gosActual.transform.position, gosActual.transform.rotation);
                        Destroy(gosActual);
                    }
                    //obj.GetComponent<Interactuable>().Interactua(1);
                } 
            }
            else if (hit.collider.CompareTag("Interactuable") || ((hit.collider.gameObject.layer == LayerMask.NameToLayer("Family Member")) && hit.collider.transform.GetComponent<Interactuable>() != null))
            {
                hit.collider.transform.GetComponent<Interactuable>().Interactua(0);
            }

            //Si té el tag "Additive" (botiquins, escut, munició)... sumem els valors en el seu script corresponent
            /*if (hit.collider.CompareTag("Additive")){
                
                Additive add = hit.collider.gameObject.GetComponent<Additive>();
                float sheildAmount = add.sheildAmount;
                float healthAmount = add.healthAmount;
                float ammoAmount = add.ammoAmount;

                Destroy(hit.collider.gameObject);

                Health playerHealth = GetComponent<Health>();
                playerHealth.Healing(healthAmount);
                playerHealth.HealingShield(sheildAmount);
            }*/
        }
    }

    void MostrarUIObjecte(GameObject obj, bool enabled)
    {
        ItemUI itemUI = obj.GetComponent<ItemUI>();
        if(itemUI != null) itemUI.itemPickedUp = enabled;
        if(itemUI != null)
        {
            itemUI.ShowAmunitionText(enabled);
        }
    }

    public void DropObject(){
        obj.transform.parent = null;
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        obj.GetComponent<BoxCollider>().enabled = true;

        MostrarUIObjecte(obj, false);

        if(obj.GetComponent<Animator>() != null) obj.GetComponent<Animator>().enabled = false;

        if(obj.transform.tag == "Os Gos")
        {
            playerTeOs = false;
            GameObject gos = GameObject.FindGameObjectWithTag("Gos");
            if(gos != null) gos.GetComponent<GosMovement>().TornarAGosNormal();
        } 

        hasObject = false;
        obj = null;       
    }

    public void DropOs(){
        obj.transform.parent = null;
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        obj.GetComponent<BoxCollider>().enabled = true;

        MostrarUIObjecte(obj, false);

        if(obj.GetComponent<Animator>() != null) obj.GetComponent<Animator>().enabled = false;

        if(obj.transform.tag == "Os Gos")
        {
            playerTeOs = false;
        } 

        hasObject = false;
        obj = null;       
    }

    void TryStartDrag()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, dragDistance))
        {
            if (hit.collider.gameObject.layer == 12) // Si te la Layer de Arrossegar
            {
                draggedObject = hit.collider.transform;
                draggedRb = draggedObject.GetComponent<Rigidbody>();

                if (draggedRb == null)
                {
                    // Afegir Rigidbody al pare
                    draggedRb = draggedObject.gameObject.AddComponent<Rigidbody>();
                }

                draggedRb.isKinematic = false;
                draggedRb.useGravity = false;
                draggedRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

                // Desactivar tots els MeshColliders fills
                MeshCollider[] childColliders = draggedObject.GetComponentsInChildren<MeshCollider>();
                foreach (var mc in childColliders)
                {
                    mc.enabled = false;
                }

                offset = draggedRb.position - hit.point;
                arrossegaObject = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (draggedRb != null && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 targetPoint = ray.origin + ray.direction * dragDistance;
            Vector3 targetPosition = targetPoint + offset;

            targetPosition.y = draggedRb.position.y;

            Vector3 force = (targetPosition - draggedRb.position) * 150f; // 50 = força de tir
            draggedRb.velocity = Vector3.zero; // evitar velocitat residual
            draggedRb.angularVelocity = Vector3.zero; // evitar rotació incontrolada
            draggedRb.AddForce(force, ForceMode.Force);
        }
    }

    void StopDrag()
    {
        if (arrossegaObject)
        {
            // Reactivar MeshColliders fills
            MeshCollider[] childColliders = draggedObject.GetComponentsInChildren<MeshCollider>();
            foreach (var mc in childColliders)
            {
                mc.enabled = true;
            }

            // Tornar Rigidbody a estat original
            Destroy(draggedRb);
            draggedRb = null;
            draggedObject = null;
            arrossegaObject = false;
        }
    }
}
