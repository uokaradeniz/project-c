using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    Rigidbody rb;
    public int projectileSpeed;
    Vector3 dir;
    float destroyTimer;
    ParticleSystem projectilePS;
    ParticleSystem projectileDestroy;
    int turretMinDmg = 2;
    int turretMaxDmg = 8;
    bool damageDone;

    // Start is called before the first frame update
    void Start()
    {
        projectilePS = transform.Find("Projectile").GetComponent<ParticleSystem>();
        rb = GetComponent<Rigidbody>();
        projectileDestroy = transform.Find("ProjectileDestroy").GetComponent<ParticleSystem>();
        projectilePS.Play();
    }

    // Update is called once per frame
    void Update()
    {
        destroyTimer += Time.deltaTime;
        if(destroyTimer >= 3.25f) {
            rb.isKinematic = true;
            projectileDestroy.Play();
        }
            GameObject.Destroy(gameObject, 3.5f);
        rb.velocity = transform.forward * projectileSpeed * Time.unscaledDeltaTime;
    }

    void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Projectile") && !other.CompareTag("Enemy") && !other.CompareTag("Ignore Projectile")) {
            rb.isKinematic = true;
            projectileDestroy.Play();
            GameObject.Destroy(gameObject, 0.2f);
        }

        if(other.CompareTag("Player") && !damageDone) {
            other.GetComponent<PlayerCombat>().playerHealth -= Random.Range(turretMinDmg, turretMaxDmg);
            damageDone = true;
            GameObject.Destroy(gameObject);
        }
    }
}
