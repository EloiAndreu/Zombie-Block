using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//COMPORTAMENT DE LA CÀMERA
public class CameraController : MonoBehaviour
{
    public float sensX, senxY;
    float xRotation, yRotation;

    public Transform orientation;
    //public GameObject objToRotateX;

    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //Determinem la rotació en funció dels inputs del ratolí
    void Update(){
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senxY;
    
        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 45f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        //objToRotateX.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
