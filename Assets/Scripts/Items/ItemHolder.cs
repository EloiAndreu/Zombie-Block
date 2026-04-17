using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [Header("Player Reference")]
    public Rigidbody playerRb;

    [Header("Position Sway (movement)")]
    public float positionSwayAmount = 0.05f;
    public float positionSwaySmooth = 5f;

    [Header("Rotation Sway (mouse look)")]
    public float rotationSwayAmount = 5f;
    public float rotationSwaySmooth = 8f;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;

    void Start()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
    }

    void Update()
    {
        // ----------------------------
        // 1. MOVEMENT SWAY (RIGIDBODY)
        // ----------------------------
        Vector3 localVelocity = transform.parent.InverseTransformDirection(playerRb.velocity);

        Vector3 targetPosition = initialLocalPosition + new Vector3(
            -localVelocity.x,
            0f,
            -localVelocity.z
        ) * positionSwayAmount;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * positionSwaySmooth
        );

        // ----------------------------
        // 2. LOOK SWAY (MOUSE)
        // ----------------------------
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion swayRotation = Quaternion.Euler(
            -mouseY * rotationSwayAmount,
            mouseX * rotationSwayAmount,
            -mouseX * rotationSwayAmount
        );

        Quaternion targetRotation = initialLocalRotation * swayRotation;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * rotationSwaySmooth
        );
    }
}