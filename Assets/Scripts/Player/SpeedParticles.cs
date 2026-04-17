using UnityEngine;

public class SpeedParticles : MonoBehaviour
{
    [Header("Referències")]
    public Rigidbody playerRb;          // o el teu controller
    public ParticleSystem ps;

    [Header("Configuració")]
    public float speedThreshold = 8f;   // velocitat mínima per activar
    public float smooth = 5f;           // suavitzat d’entrada/sortida

    private float currentIntensity = 0f;
    private ParticleSystem.EmissionModule emission;

    void Start()
    {
        emission = ps.emission;
        emission.rateOverTime = 0f;
    }

    void Update()
    {
        float speed = playerRb.velocity.magnitude; // usa velocity si és Rigidbody clàssic

        // 0 si lent, 1 si ràpid
        float target = speed > speedThreshold ? 1f : 0f;

        // suavitzat per evitar canvis bruscos
        currentIntensity = Mathf.Lerp(currentIntensity, target, Time.deltaTime * smooth);

        // controla emissió
        emission.rateOverTime = currentIntensity * 100f;

        // opcional: play/stop per estalviar recursos
        if (currentIntensity > 0.05f && !ps.isPlaying)
            ps.Play();

        if (currentIntensity <= 0.05f && ps.isPlaying)
            ps.Stop();
    }
}