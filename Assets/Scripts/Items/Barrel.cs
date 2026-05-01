using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject explossionParticles;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public float upwardsModifier = 1.5f;

    public float explosionMinDamage = 30f;
    public float explosionMaxDamage = 150f;

    private bool hasExploded = false;

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Vector3 explosionPosition = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);

        // Partícules
        Instantiate(explossionParticles, explosionPosition, transform.rotation);

        // Detectar objectes propers
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

            // 📏 Calcular distància
            float distance = Vector3.Distance(explosionPosition, nearbyObject.transform.position);

            // Normalitzar (0 = centre, 1 = límit del radi)
            float t = distance / explosionRadius;

            // Invertir perquè més aprop = més dany
            float damage = Mathf.Lerp(explosionMaxDamage, explosionMinDamage, t);

            // Aplicar dany si té Health
            Health health = nearbyObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage, rb, Vector3.zero);
            }

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
        }

        Destroy(this.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 explosionPosition = new Vector3(
            transform.position.x,
            transform.position.y + 2.5f,
            transform.position.z
        );

        Gizmos.DrawWireSphere(explosionPosition, explosionRadius);
    }
}