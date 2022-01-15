using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [HideInInspector] public float remainingBulletsPistol;
    [HideInInspector] public int remainingBulletsRifle;
    [HideInInspector] public int remainingBulletsPC;

    public int playerStartHealth;
    public int playerHealth;
    public float fallDmgMultiplier;

    float fireCooldown;
    float shootTimer;
    public float rifleFireCooldown;
    public float knockForce;

    public int pistolMagSize;
    public int rifleMagSize;
    int magSize;
    public bool isReloading;

    [HideInInspector] public int selectedWeapon;

    public bool playerDied;
    public bool godMode;
    public bool playerInvisible;

    public GameObject pistol;
    public float pistolCDRMultiplier;

    public GameObject rifle;
    public GameObject knife;
    public GameObject plasmaCannon;
    Animator animator;
    ParticleSystem reloadAnimPistol;
    ParticleSystem reloadAnimRifle;

    FPController fPController;
    [HideInInspector] public GameSettings gameSettings;

    public enum weaponHolderEnum
    {
        EnergyPistol,
        EnergyRifle,
        PlasmaCannon,
        Knife
    }
    public weaponHolderEnum weaponHolder;

    // Start is called before the first frame update
    void Start()
    {
        fPController = GetComponent<FPController>();
        reloadAnimPistol = transform.Find("Main Camera/WeaponInventory/EnergyPistol/ChargeP").GetComponent<ParticleSystem>();
        reloadAnimRifle = transform.Find("Main Camera/WeaponInventory/EnergyRifle/ChargeR").GetComponent<ParticleSystem>();
        gameSettings = GameObject.Find("Game Settings").GetComponent<GameSettings>();
        animator = GetComponent<Animator>();
        playerHealth = playerStartHealth;
        remainingBulletsPistol = pistolMagSize;
        remainingBulletsRifle = rifleMagSize;
        selectedWeapon = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && !godMode)
            godMode = true;
        else if (Input.GetKeyDown(KeyCode.G) && godMode)
            godMode = false;

        if (godMode)
            playerHealth = playerStartHealth;

        if (playerHealth > 100)
            playerHealth = 100;

        if (playerHealth <= 0 && !gameSettings.levelCleared)
        {
            playerDied = true;
            playerHealth = 0;
            transform.Find("Main Camera/WeaponInventory").gameObject.SetActive(false);
        }

        if (gameSettings.levelCleared)
        {
            transform.Find("Main Camera/WeaponInventory").gameObject.SetActive(false);
        }

        if (!playerDied && !gameSettings.levelCleared)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                isReloading = false;
                selectedWeapon = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                isReloading = false;
                selectedWeapon = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                isReloading = false;
                selectedWeapon = 3;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                isReloading = false;
                selectedWeapon = 4;
            }

            if (remainingBulletsPistol > 0)
                remainingBulletsPistol += Time.deltaTime * pistolCDRMultiplier;
            if (remainingBulletsPistol >= 11)
                remainingBulletsPistol = 11;
            if (remainingBulletsPistol < 0)
                remainingBulletsPistol = 0;


            switch (selectedWeapon)
            {
                case 1:
                    weaponHolder = weaponHolderEnum.EnergyPistol;
                    break;
                case 2:
                    weaponHolder = weaponHolderEnum.EnergyRifle;
                    break;
                case 3:
                    weaponHolder = weaponHolderEnum.Knife;
                    break;
                case 4:
                    weaponHolder = weaponHolderEnum.PlasmaCannon;
                    break;
            }

            if (weaponHolder == weaponHolderEnum.EnergyPistol)
            {
                fPController.canDoubleJump = false;
                pistol.gameObject.SetActive(true);
                rifle.gameObject.SetActive(false);
                knife.gameObject.SetActive(false);
                plasmaCannon.gameObject.SetActive(false);
                magSize = pistolMagSize;

                if (Input.GetButtonDown("Fire1") && remainingBulletsPistol > 0 && !isReloading)
                {
                    animator.SetTrigger("Fire");
                }
            }
            else if (weaponHolder == weaponHolderEnum.EnergyRifle)
            {
                fPController.canDoubleJump = false;
                pistol.gameObject.SetActive(false);
                rifle.gameObject.SetActive(true);
                knife.gameObject.SetActive(false);
                plasmaCannon.gameObject.SetActive(false);
                magSize = rifleMagSize;

                if (Input.GetButton("Fire1") && remainingBulletsRifle > 0 && !isReloading)
                {
                    animator.SetTrigger("Fire");
                }
            }
            else if (weaponHolder == weaponHolderEnum.Knife)
            {
                fPController.canDoubleJump = true;
                pistol.gameObject.SetActive(false);
                rifle.gameObject.SetActive(false);
                knife.gameObject.SetActive(true);
                plasmaCannon.gameObject.SetActive(false);
                magSize = 0;
                if (Input.GetButton("Fire1"))
                {
                    animator.SetBool("KnifeAtk", true);
                }
            }
            else if (weaponHolder == weaponHolderEnum.PlasmaCannon)
            {
                fPController.canDoubleJump = false;
                pistol.gameObject.SetActive(false);
                rifle.gameObject.SetActive(false);
                knife.gameObject.SetActive(false);
                plasmaCannon.gameObject.SetActive(true);
                if (Input.GetButtonDown("Fire1") && remainingBulletsPC > 0)
                {
                    animator.SetTrigger("Fire");
                }
            }

            if (playerInvisible)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var enemy in enemies)
                {
                    Collider[] colliders = Physics.OverlapSphere(enemy.transform.position, 8);
                    foreach (var collider in colliders)
                    {
                        if (collider.CompareTag("Player"))
                            enemy.GetComponent<EnemyAI>().stopAI = false;
                        else
                            enemy.GetComponent<EnemyAI>().stopAI = true;
                    }
                }
            }
            else
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var enemy in enemies)
                {
                    enemy.GetComponent<EnemyAI>().stopAI = false;
                }
            }


            if (Input.GetButtonDown("Reload") && !isReloading)
            {
                if (weaponHolder == weaponHolderEnum.EnergyPistol && remainingBulletsPistol != magSize && remainingBulletsPistol <= 0)
                {
                    reloadAnimPistol.Play();
                    animator.SetTrigger("ReloadPistol");
                    isReloading = true;
                }
                else if (weaponHolder == weaponHolderEnum.EnergyRifle && remainingBulletsRifle != magSize)
                {
                    reloadAnimRifle.Play();
                    animator.SetTrigger("ReloadRifle");
                    isReloading = true;
                }
            }
        }
    }

    void Reload()
    {
        isReloading = false;
        if (weaponHolder == weaponHolderEnum.EnergyPistol)
            remainingBulletsPistol = magSize;
        else if (weaponHolder == weaponHolderEnum.EnergyRifle)
            remainingBulletsRifle = magSize;
    }

    void KnifeReset()
    {
        animator.SetBool("KnifeAtk", false);
    }

    void KnifeAtk()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.Find("Main Camera/WeaponInventory/AtkPivot").position, 1.35f, 1 << 7);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                collider.transform.Find("HitSparks").GetComponent<ParticleSystem>().Play();
                var enemyAI = collider.GetComponent<EnemyAI>();
                if (enemyAI.enemyType != EnemyAI.EnemyType.Turret)
                    collider.GetComponent<Rigidbody>().AddForce(transform.forward * 5 + transform.up * 3, ForceMode.Impulse);
                enemyAI.enemyHealth -= Random.Range(30, 60);
                enemyAI.enemyGotHit = true;
                enemyAI.ehbTimer = 0;
            }
        }
    }

    public void FirePistol()
    {
        Instantiate(Resources.Load("EnergyPistolProjectile"));
        remainingBulletsPistol--;
    }

    public void FireRifle()
    {
        Instantiate(Resources.Load("EnergyRifleProjectile"));
        remainingBulletsRifle--;
    }

    public void FirePlasmaCannon()
    {
        Instantiate(Resources.Load("PlasmaCannonProjectile"));
        remainingBulletsPC--;
    }

    public void RifleTriggerTwiceFix()
    {
        animator.ResetTrigger("Fire");
    }
}
