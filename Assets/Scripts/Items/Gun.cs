using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//COMPORTAMENT DE L'ARMA (Jugador)
public class Gun : MonoBehaviour
{
    public int gunID;

    [Header("Propietats")]
    public float shootMaxDist = 500f;
    public float damage = 20f;
    public float fireRate = 0.5f;
    bool canShoot = true;
    public int maxBales = 10;
    public int currentBales = 10;
    public float timeReload = 5f;
    public float impactForce = 20f;
    Coroutine reloadCoroutine = null;
    [SerializeField] LayerMask shootMask;
    public bool canAim = true;
    public bool automaticFire = false;
    bool apuntPerDisparar = false;

    public GameObject shootPoint;

    [Header("Elements externs")]
    public GameObject balaPrefab;//, particlesBlood;
    Animator anim;
    public ParticleSystem muzzleFlash;

    [Header("Shotgun Settings")]
    public int nBales = 1;  
    public float spread = 0.02f;
    public bool diferentAnimations = false;
    
    void Start(){
        currentBales = maxBales;
        anim = GetComponent<Animator>();
    }

    void Update(){
        //APUNTAR
        if (Input.GetMouseButton(1)){
            if(reloadCoroutine != null){
                StopCoroutine(reloadCoroutine);
                reloadCoroutine = null;
            }
            //GetComponent<BoxCollider>().enabled = false;
            if(canAim && !diferentAnimations) anim.SetFloat("State", 1f); //Animació apuntar
        }
        else{
            //GetComponent<BoxCollider>().enabled = true;
            if(reloadCoroutine == null){
               if(!diferentAnimations) anim.SetFloat("State", 0.5f); //Animació idle
            }
            else{
               if(!diferentAnimations) anim.SetFloat("State", 0f); //Animació recarregar
            }
        }

        if(!automaticFire && Input.GetMouseButtonDown(0) && apuntPerDisparar)
        {
            apuntPerDisparar = false;
            canShoot = true;
        }
    }

    //Mètode públic per permetre disparar desde altres scripts
    public void Shoot(){
        if(canShoot){
            StartCoroutine(ShootWithCooldown());
        }
    }

    IEnumerator ShootWithCooldown()
    {
        canShoot = false;
        if(currentBales > 0) ShootGun();

        //Esperem 'fireRate' segons abans de tornar a disparar
        yield return new WaitForSeconds(fireRate);
        apuntPerDisparar = true;
        if(automaticFire) canShoot = true;
    }

    //DISPARAR
    /*void ShootGun(){
        if(Camera.main.transform.parent.GetComponent<CameraShake>() != null) StartCoroutine(Camera.main.transform.parent.GetComponent<CameraShake>().Shake(0.05f, impactForce/100f));

        currentBales--;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (!Input.GetMouseButton(1)) anim.SetTrigger("Shoot");            

        if (Physics.Raycast(ray, out hit, shootMaxDist, shootMask)){
            //Generem una element de partícules per simular la colisió de la bala.
            Instantiate(balaPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            
            Health health = hit.collider.GetComponentInParent<Health>();
            if (health != null)
            {
                if(particlesBlood != null && hit.collider.gameObject.layer != 9) Instantiate(particlesBlood, hit.point, Quaternion.LookRotation(hit.normal));
                Rigidbody rb = hit.collider.attachedRigidbody;
                Vector3 force = -hit.normal * impactForce;

                health.TakeDamage(damage, rb, force);
            }
        }
    }*/

    /*void ShootGun()
    {
        currentBales--;

        if (!Input.GetMouseButton(1))
            anim.SetTrigger("Shoot");

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        RaycastHit hit;
        Vector3 targetPoint;

        // 🔥 Si col·lisiona → disparem exactament al punt d’impacte
        if (Physics.Raycast(ray, out hit, shootMaxDist, shootMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            // Si no toca res → punt llunyà
            targetPoint = ray.GetPoint(shootMaxDist);
        }

        // Direcció real des del canó fins al punt objectiu
        Vector3 direction = (targetPoint - shootPoint.transform.position).normalized;

        // Crear bala mirant cap a l’objectiu
        GameObject bullet = Instantiate(balaPrefab, shootPoint.transform.position, Quaternion.LookRotation(direction));

        // Passar valors
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.damage = damage;
        bulletScript.impactForce = impactForce;

        if (Camera.main.transform.parent.GetComponent<CameraShake>() != null)
            StartCoroutine(Camera.main.transform.parent.GetComponent<CameraShake>().Shake(0.05f, impactForce / 100f));
    }*/

    void ShootGun()
    {
        currentBales--;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, shootMaxDist, shootMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(shootMaxDist);
        }

        if(Vector3.Distance(shootPoint.transform.position, hit.point) < 15f)
        {
            targetPoint = ray.GetPoint(shootMaxDist);
        }

        //  DISPARAR múltiples bales
        for (int i = 0; i < nBales; i++)
        {
            // Direcció base
            Vector3 direction = (targetPoint - shootPoint.transform.position).normalized;

            //  Afegim dispersió aleatòria
            direction += new Vector3(
                Random.Range(-spread, spread),
                Random.Range(-spread, spread),
                Random.Range(-spread, spread)
            );

            direction.Normalize();

            muzzleFlash.Play();

            GameObject bullet = Instantiate(
                balaPrefab,
                shootPoint.transform.position,
                Quaternion.LookRotation(direction)
            );

            Bullet bulletScript = bullet.GetComponent<Bullet>();

            //  Repartim el damage
            bulletScript.damage = damage / nBales;
            bulletScript.impactForce = impactForce / nBales;
        }

        if (Camera.main.transform.parent.GetComponent<CameraShake>() != null)
            StartCoroutine(Camera.main.transform.parent.GetComponent<CameraShake>().Shake(0.05f, impactForce / 100f));
        
        if (!Input.GetMouseButton(1))
            anim.SetTrigger("Shoot");
    }

    //Mètode públic per permetre recarregar desde altres scripts
    public void Reload(){
        if(reloadCoroutine==null && currentBales < maxBales){
            reloadCoroutine = StartCoroutine(ReloadGun());
        }
    }

    //RECARREGAR
    IEnumerator ReloadGun(){
        if(!diferentAnimations) anim.SetFloat("State", 0);
        yield return new WaitForSeconds(timeReload); 
        currentBales = maxBales;
        reloadCoroutine = null;
    }
}
