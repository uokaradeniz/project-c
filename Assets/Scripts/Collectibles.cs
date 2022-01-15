using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
    bool packAcquired;
    public float rotateSpeed;

    public enum CollectibleType
    {
        HealthPack,
        PlasmaCannonCharge
    }
    public CollectibleType collectibleType;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    { 
        var mesh = transform.Find("CollectibleMesh");
        mesh.Rotate(Time.deltaTime * rotateSpeed, Time.deltaTime * rotateSpeed, Time.deltaTime * rotateSpeed);

        if (collectibleType == CollectibleType.HealthPack)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.4f);

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player") && !packAcquired && collider.GetComponent<PlayerCombat>().playerHealth < 100)
                {
                    collider.GetComponent<PlayerCombat>().playerHealth += Random.Range(12, 30);
                    packAcquired = true;
                    GameObject.Destroy(gameObject);
                }
            }
        }

        if (collectibleType == CollectibleType.PlasmaCannonCharge)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.4f);

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player") && !packAcquired)
                {
                    collider.GetComponent<PlayerCombat>().remainingBulletsPC++;
                    packAcquired = true;
                    GameObject.Destroy(gameObject);
                }
            }
        }
    }
}
