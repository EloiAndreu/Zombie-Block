using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 100f;
    public float lifeTime = 3f;
    public float damage = 10;
    public float impactForce = 50f;

    public GameObject particlesBlood, particleMuzzleFlash, particlesSparks;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    /*void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }*/

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        Vector3 hitPoint = contact.point;
        Vector3 hitNormal = contact.normal;

        Health health = collision.gameObject.GetComponentInParent<Health>();

        if (health != null)
        {
            Rigidbody rb = collision.rigidbody;
            Vector3 force = -hitNormal * impactForce;

            health.TakeDamage(damage, rb, force);

            // Partícules de sang
            if (particlesBlood != null && collision.gameObject.layer != 9)
            {
                Instantiate(particlesBlood, hitPoint, Quaternion.LookRotation(hitNormal));
            }
            else if (particleMuzzleFlash != null)
            {
                Instantiate(particleMuzzleFlash, hitPoint, Quaternion.LookRotation(hitNormal));
            }

            if (particlesSparks != null)
            {
                Instantiate(particlesSparks, hitPoint, Quaternion.LookRotation(hitNormal));
            }
        }
        else
        {
            if (particleMuzzleFlash != null)
            {
                Instantiate(particleMuzzleFlash, hitPoint, Quaternion.LookRotation(hitNormal));
            }

            if (particlesSparks != null)
            {
                Instantiate(particlesSparks, hitPoint, Quaternion.LookRotation(hitNormal));
            }
        }

        Destroy(gameObject);
    }
}