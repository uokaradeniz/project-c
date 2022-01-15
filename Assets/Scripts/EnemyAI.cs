using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public bool playerSighted;
    public bool stopAI;

    public int enemyHealth;

    int wandererHealth = 200;
    int droneHealth = 75;
    int dummyHealth = 200;
    int megabotHealth = 400;

    //Turret
    int turretHealth = 500;
    float shootTimer;
    float shootInterval = 0.4f;
    float rotSpeed = 4.5f;
    Transform muzzle;
    Transform turretHead;
    GameObject player;

    float megabotChargeTimer;
    bool isCharged;
    bool canCC;

    Slider enemyHealthBar;
    [HideInInspector] public bool enemyGotHit;
    [HideInInspector] public float ehbTimer;

    NavMeshAgent navMeshAI;
    float wanderTime;
    public float wanderPathDuration;

    public enum EnemyType
    {
        Turret,
        Wanderer,
        Drone,
        Dummy,
        Megabot
    }

    public EnemyType enemyType;

    void Start()
    {

        player = GameObject.Find("Player");
        enemyHealthBar = transform.Find("EnemyHealthBar/HealthBar").GetComponent<Slider>();

        if (enemyType == EnemyType.Turret)
        {
            enemyHealth = turretHealth;
            turretHead = transform.Find("TurretBody");
            muzzle = transform.Find("TurretBody/MuzzleEnemy");
        }

        if (enemyType == EnemyType.Wanderer)
        {
            navMeshAI = GetComponent<NavMeshAgent>();
            enemyHealth = wandererHealth;
            muzzle = transform.Find("WandererMesh/Hands/Weapon/MuzzleEnemy");
        }

        if (enemyType == EnemyType.Drone)
        {
            muzzle = transform.Find("Body/Muzzle");
            enemyHealth = droneHealth;
            navMeshAI = GetComponent<NavMeshAgent>();
        }

        if (enemyType == EnemyType.Dummy)
        {
            enemyHealth = dummyHealth;
        }

        if (enemyType == EnemyType.Megabot)
        {
            enemyHealth = megabotHealth;
            navMeshAI = GetComponent<NavMeshAgent>();
            muzzle = transform.Find("Gun/Muzzle");

        }
    }

    void Update()
    {
        if (player.GetComponent<PlayerCombat>().playerDied)
            stopAI = true;

        if (enemyHealth <= 0)
        {
            enemyHealth = 0;
            GameObject.Destroy(gameObject, .2f);
        }

        if (enemyGotHit)
        {
            ehbTimer += Time.deltaTime;
        }

        if (ehbTimer >= 4)
        {
            enemyGotHit = false;
            ehbTimer = 0;
        }

        transform.Find("EnemyHealthBar").GetComponent<Animator>().SetBool("FadeAnim", enemyGotHit);

        if (!stopAI)
        {
            if (enemyType == EnemyType.Turret)
            {
                enemyHealthBar.maxValue = turretHealth;
                TurretAI();
            }

            if (enemyType == EnemyType.Dummy)
            {
                enemyHealthBar.maxValue = dummyHealth;
                DummyAI();
            }
            if (enemyType == EnemyType.Wanderer)
            {
                enemyHealthBar.maxValue = wandererHealth;
                WandererAI();
            }

            if (enemyType == EnemyType.Drone)
            {
                enemyHealthBar.maxValue = droneHealth;
                DroneAI();
            }

            if (enemyType == EnemyType.Megabot)
            {
                enemyHealthBar.maxValue = megabotHealth;
                MegabotAI();
            }
        }

        enemyHealthBar.transform.parent.LookAt(Camera.main.transform.position);
        enemyHealthBar.value = enemyHealth;
    }


    void TurretAI()
    {
        Collider[] stepOnFix = Physics.OverlapSphere(transform.position + new Vector3(0, 2, 0), 1, 1 << 9);
        foreach (var obj in stepOnFix)
        {
            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<CharacterController>().Move(obj.transform.forward * .04f);
            }
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, 40f, 1 << 9);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
                playerSighted = true;
            else
                playerSighted = false;

            if (playerSighted)
            {
                RaycastHit hit;
                if (Physics.Raycast(turretHead.position, collider.transform.position - turretHead.position, out hit, 40f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        var rot = Quaternion.LookRotation((collider.transform.position - new Vector3(0, 1.65f, 0)) - turretHead.transform.position);
                        turretHead.transform.rotation = Quaternion.Slerp(turretHead.transform.rotation, rot, Time.deltaTime * rotSpeed);
                        muzzle.transform.rotation = Quaternion.Slerp(muzzle.transform.rotation, rot, Time.deltaTime * rotSpeed);
                        shootTimer += Time.deltaTime;
                        if (shootTimer >= shootInterval)
                        {
                            Instantiate(Resources.Load("EnemyProjectile"), muzzle.position, muzzle.transform.rotation);
                            shootTimer = 0;
                        }
                    }
                }
            }
        }
    }

    public Vector3 WanderRandomDirection(Vector3 origin, float distance, int layerMask)
    {
        Vector3 randomDir = Random.insideUnitSphere * distance;
        randomDir += origin;
        NavMesh.SamplePosition(randomDir, out NavMeshHit navMeshHit, distance, layerMask);
        return navMeshHit.position;
    }

    void WandererAI()
    {
        Collider[] stepOnFix = Physics.OverlapSphere(transform.position + new Vector3(0, 1, 0), 0.5f, 1 << 9);
        foreach (var obj in stepOnFix)
        {
            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<CharacterController>().Move(obj.transform.forward * .04f);
            }
        }

        wanderTime += Time.fixedDeltaTime * wanderPathDuration;

        if (wanderTime >= 3)
        {
            Vector3 newPos = WanderRandomDirection(transform.position, 15, -1);
            navMeshAI.SetDestination(newPos);
            wanderTime = 0;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, 40))
        {
            if (hit.collider.CompareTag("Player"))
                playerSighted = true;
            else
                playerSighted = false;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) < 20)
            playerSighted = true;
        else if (Vector3.Distance(transform.position, player.transform.position) > 40)
            playerSighted = false;

        if (playerSighted)
        {
            Transform hands = transform.Find("WandererMesh/Hands");
            navMeshAI.angularSpeed = 0;
            if (Vector3.Distance(transform.position, player.transform.position) < 4.5f)
            {
                navMeshAI.SetDestination(Vector3.MoveTowards(transform.position, player.transform.position, -navMeshAI.speed));
                navMeshAI.speed = 4;
                shootInterval = 2;
            }
            else
            {
                navMeshAI.speed = 6;
                shootInterval = 0.4f;
            }
            Quaternion lookRot = Quaternion.LookRotation((player.transform.position - new Vector3(0, 1.25f, 0)) - transform.position);
            hands.rotation = Quaternion.RotateTowards(hands.rotation, lookRot, rotSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, rotSpeed);
            muzzle.LookAt(player.transform.position);
            shootTimer += Time.deltaTime;



            if (shootTimer > shootInterval)
            {
                Instantiate(Resources.Load("EnemyProjectile"), muzzle.position, transform.rotation);
                shootTimer = 0;
            }
        }
        else
        {
            navMeshAI.angularSpeed = 120;
            navMeshAI.speed = 3.5f;
        }
    }

    void DummyAI()
    {
        Collider[] stepOnFix = Physics.OverlapSphere(transform.position + new Vector3(0, 1, 0), 0.5f, 1 << 9);
        foreach (var obj in stepOnFix)
        {
            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<CharacterController>().Move(obj.transform.forward * .04f);
            }
        }
    }

    void DroneAI()
    {
        Collider[] stepOnFix = Physics.OverlapSphere(transform.position + new Vector3(0, 0.5f, 0), 1, 1 << 9);
        foreach (var obj in stepOnFix)
        {
            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<CharacterController>().Move(obj.transform.forward * .04f);
            }
        }

        if (Vector3.Distance(transform.position, player.transform.position) < 25)
            playerSighted = true;
        else
            playerSighted = false;

        if (playerSighted)
        {
            navMeshAI.SetDestination(player.transform.position);
            shootTimer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position + new Vector3(0, 0.25f, 0) - transform.position), rotSpeed);
            RaycastHit hit;

            if (Physics.Raycast(muzzle.transform.position, muzzle.transform.forward, out hit, 15f))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    if (shootTimer > shootInterval + .5f)
                    {
                        Instantiate(Resources.Load("EnemyProjectile"), muzzle.position, transform.rotation);
                        shootTimer = 0;
                    }
                }
            }
        }
        else
        {
            wanderTime += Time.fixedDeltaTime * wanderPathDuration;

            if (wanderTime >= 3)
            {
                Vector3 newPos = WanderRandomDirection(transform.position, 15, -1);
                navMeshAI.SetDestination(newPos);
                wanderTime = 0;
            }
        }
    }

    void MegabotAI()
    {
        Collider[] stepOnFix = Physics.OverlapSphere(transform.position + new Vector3(0, 2, 0), 1, 1 << 9);
        foreach (var obj in stepOnFix)
        {
            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<CharacterController>().Move(obj.transform.forward * .05f);
            }
        }

        RaycastHit hitTrigger;
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hitTrigger, 30))
        {
            if (hitTrigger.collider.CompareTag("Player"))
                playerSighted = true;
            else
                playerSighted = false;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) < 5)
        {
            playerSighted = true;
        }

        if (Vector3.Distance(transform.position, player.transform.position) <= 3.5f)
            canCC = true;
        else
            canCC = false;

        if (playerSighted)
        {
            int randomizeChargeAttack = Random.Range(0, 50);
            if (randomizeChargeAttack >= 49 && !isCharged)
            {
                isCharged = true;
                navMeshAI.speed = navMeshAI.speed * 2.5f;
                Invoke("MegabotStopChargeAttack", 4);
            }

            if (isCharged)
                megabotChargeTimer += Time.deltaTime;
            else if (megabotChargeTimer >= 12)
            {
                isCharged = false;
                megabotChargeTimer = 0;
            }

            navMeshAI.SetDestination(player.transform.position);
            transform.Find("Body").rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), rotSpeed);
            transform.Find("Gun").rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position + new Vector3(0, -.5f, 0) - transform.position), rotSpeed);
            muzzle.LookAt(player.transform.position);

            if (!canCC)
            {
                shootTimer += Time.deltaTime;
                RaycastHit hit;

                if (Physics.Raycast(muzzle.transform.position, muzzle.transform.forward, out hit, 15f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        if (shootTimer > shootInterval + .25f)
                        {
                            Instantiate(Resources.Load("EnemyProjectile"), muzzle.position, muzzle.rotation);
                            shootTimer = 0;
                        }
                    }
                }
            }
            GetComponent<Animator>().SetBool("canCC", canCC);
        }

        wanderTime += Time.fixedDeltaTime * wanderPathDuration;

        if (wanderTime >= 3)
        {
            Vector3 newPos = WanderRandomDirection(transform.position, 15, -1);
            navMeshAI.SetDestination(newPos);
            wanderTime = 0;
        }

        if (navMeshAI.velocity.magnitude > 0)
            GetComponent<Animator>().SetBool("isMoving", true);
        else
            GetComponent<Animator>().SetBool("isMoving", false);

    }

    void MegabotStopChargeAttack()
    {
        navMeshAI.speed = navMeshAI.speed / 2.5f;
    }

    void MegabotKnifeAtk()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + new Vector3(0, 0, 3), 6, 1 << 9);

        foreach (var collider in colliders)
        {
            bool atkApplied = false;

            if (collider.CompareTag("Player") && !atkApplied)
            {
                if (!isCharged)
                    collider.GetComponent<PlayerCombat>().playerHealth -= Random.Range(5, 10);
                else
                    collider.GetComponent<PlayerCombat>().playerHealth -= Random.Range(10, 20);
                atkApplied = true;
            }
        }
    }
}