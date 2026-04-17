using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Moviment")]
    public float moveSpeed;
    public float speedMultiplier = 1.5f;
    float multiplier=1f;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Inputs")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode reloadKey = KeyCode.R;
    float horizontalInput, verticalInput;

    [Header("Ground Check")]
    public float playerHeight;
    public bool grounded;
    public LayerMask groundLayer;

    [Header("Elements externs")]
    public Transform orientation;
    Vector3 moveDirection;
    Rigidbody rb;
    //public Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }

    private void Update()
    {
        //Comprovem si el jugador toca una layer caminable (Ground)
        //LayerMask groundLayer = LayerMask.GetMask("Ground");
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundLayer);

        MyInput(); 
        SpeedControl();

        // Gestionem el drag
        if (grounded){
            rb.drag = groundDrag;
        }
        else{
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //float currentAnimValue = anim.GetFloat("Movement");
        //float smoothSpeed = 5f;

        //Apliquem animacions i multiplicadors de velocitat en funció dels Inputs
        //Botó esquerra del ratolí... intentar apuntar (caminar més lent)
        if (Input.GetMouseButton(1)) {
            //float newMovement = Mathf.MoveTowards(currentAnimValue, 0, Time.deltaTime * smoothSpeed);
            //anim.SetFloat("Movement", newMovement);
            multiplier = 0.5f;
        } 
        else{
            if (!Input.GetMouseButton(0)){
                float targetMovement = (horizontalInput != 0f || verticalInput != 0f) ? 1f : 0.5f;

                if(Input.GetKey(runKey)){
                    multiplier = speedMultiplier;
                    targetMovement = 1.5f;
                }
                else multiplier=1f;
                
                //float newMovement = Mathf.MoveTowards(currentAnimValue, targetMovement, Time.deltaTime * smoothSpeed);
                //anim.SetFloat("Movement", newMovement);
            }
            else{
                //anim.SetFloat("Movement", 0);
            }
        }

        //Botó esquerra del ratolí... intentar agafar objecte
        if(Input.GetMouseButton(0)){
            if(GetComponent<Pickup>().obj != null){
                GameObject obj = GetComponent<Pickup>().obj;
                if(obj.GetComponent<Interactuable>()!=null){
                    obj.GetComponent<Interactuable>().Interactua(0);
                }
            }
        }

        //Botó de recarregar... intentar recarregar arma
        if(Input.GetKey(reloadKey)){
            if(GetComponent<Pickup>().obj != null){
                GameObject obj = GetComponent<Pickup>().obj;
                if(obj.GetComponent<Interactuable>()!=null){
                    obj.GetComponent<Interactuable>().Interactua(1);
                }
            }
        }

        //Comprovar si es pot saltar
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump(); //Saltar
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }


    private void MovePlayer()
    {
        // Calculem la direcció del moviment
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(grounded){ //Si està al terra... apliquem un velocitat
            rb.AddForce(moveDirection.normalized * moveSpeed * multiplier * 10f, ForceMode.Force);
        } 
        else if(!grounded){ //Si està a l'aire... apliquem una velocitat més lenta
            rb.AddForce(moveDirection.normalized * moveSpeed * multiplier * 10f * airMultiplier, ForceMode.Force);
        }
    }

    //Determinem el límits de velocitat
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed * multiplier){
            Vector3 limitedVel = flatVel.normalized * moveSpeed*multiplier;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    //SALT
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    //RESTABLIM EL SALT
    private void ResetJump()
    {
        readyToJump = true;
    }

}