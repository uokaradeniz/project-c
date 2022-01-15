using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody rb;
    public int projectileSpeed;
    GameObject muzzle;
    Vector3 dir;
    ParticleSystem projectilePS;
    ParticleSystem prjctlDestroyPS;
    float psDestroyTimer;
    GameObject player;
    bool damageDone;

    public enum ProjectileType
    {
        Normal,
        Explosive
    }

    public ProjectileType projectileType;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        muzzle = GameObject.FindGameObjectWithTag("Muzzle");
        projectilePS = gameObject.transform.Find("Projectile").GetComponent<ParticleSystem>();
        prjctlDestroyPS = gameObject.transform.Find("ProjectileDestroy").GetComponent<ParticleSystem>();
        rb = GetComponent<Rigidbody>();
        transform.position = muzzle.transform.position;
        dir = muzzle.transform.forward;
        rb.velocity = dir * Time.unscaledDeltaTime * projectileSpeed;
        projectilePS.Play();
        muzzle.GetComponent<ParticleSystem>().Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (projectileType == ProjectileType.Normal)
        {
            psDestroyTimer += Time.deltaTime;

            if (psDestroyTimer >= 0.75f)
            {
                prjctlDestroyPS.Play();
                rb.isKinematic = true;
            }
            GameObject.Destroy(gameObject, 1f);
        }

        if (projectileType == ProjectileType.Explosive)
        {
            psDestroyTimer += Time.deltaTime;
            if (psDestroyTimer >= 5)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 5);
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        collider.transform.Find("HitSparks").GetComponent<ParticleSystem>().Play();
                        collider.GetComponent<EnemyAI>().enemyHealth -= Random.Range(45, 90);
                    }
                    prjctlDestroyPS.Play();
                    GameObject.Destroy(gameObject, 1f);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (projectileType == ProjectileType.Explosive)
        {
            rb.isKinematic = true;
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5);
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    collider.transform.Find("HitSparks").GetComponent<ParticleSystem>().Play();
                    EnemyAI enemyAI = collider.GetComponent<EnemyAI>();
                    enemyAI.enemyHealth -= Random.Range(45, 90);
                    enemyAI.enemyGotHit = true;
                    enemyAI.ehbTimer = 0;
                    if (enemyAI.enemyType != EnemyAI.EnemyType.Turret)
                    {
                        collider.GetComponent<Rigidbody>().AddExplosionForce(5, transform.position, 5, 1, ForceMode.Impulse);
                    }
                }
                else if (collider.CompareTag("Player"))
                {
                    PlayerCombat playerCombat = collider.GetComponent<PlayerCombat>();
                    playerCombat.playerHealth -= Random.Range(5, 15);
                }
            }
            prjctlDestroyPS.Play();
            GameObject.Destroy(gameObject, 1f);
        }

        if (projectileType == ProjectileType.Normal)
        {
            if (!other.CompareTag("Projectile") && !other.CompareTag("Player") && !other.CompareTag("Ignore Projectile"))
            {
                rb.isKinematic = true;
                prjctlDestroyPS.Play();
                GameObject.Destroy(gameObject, 0.2f);
            }

            if (other.CompareTag("Enemy") && !damageDone)
            {
                EnemyAI enemyAI = other.GetComponent<EnemyAI>();
                enemyAI.enemyGotHit = true;
                enemyAI.ehbTimer = 0;
                other.transform.Find("HitSparks").GetComponent<ParticleSystem>().Play();
                if (player.GetComponent<PlayerCombat>().weaponHolder == PlayerCombat.weaponHolderEnum.EnergyPistol)
                    enemyAI.enemyHealth -= Random.Range(12, 28);
                if (player.GetComponent<PlayerCombat>().weaponHolder == PlayerCombat.weaponHolderEnum.EnergyRifle)
                    enemyAI.enemyHealth -= Random.Range(10, 24);
                damageDone = true;
            }
        }
    }
}
