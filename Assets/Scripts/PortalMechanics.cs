using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalMechanics : MonoBehaviour
{
    Transform portalA;
    Transform portalB;

    public bool isTeleported;
    float tpCooldown;

    // Start is called before the first frame update
    void Start()
    {
        portalA = transform.Find("PortalA");
        portalB = transform.Find("PortalB");
        ParticleSystem.MainModule mainA = portalA.GetComponent<ParticleSystem>().main;
        ParticleSystem.MainModule mainB = portalB.GetComponent<ParticleSystem>().main;
    }

    // Update is called once per frame
    void Update()
    {
        if(isTeleported) {
            tpCooldown += Time.deltaTime;
            if(tpCooldown >= 10) {
                tpCooldown = 0;
                ParticleSystem.MainModule mainA = portalA.GetComponent<ParticleSystem>().main;
                ParticleSystem.MainModule mainB = portalB.GetComponent<ParticleSystem>().main;
                mainA.startColor = Color.green;
                mainB.startColor = Color.green;
                isTeleported = false;
            }
        }

        Collider[] collidersA = Physics.OverlapSphere(portalA.transform.position, 0.4f);

        foreach (var collider in collidersA)
        {
            if(collider.CompareTag("Player") && !isTeleported) {
                collider.transform.position = portalB.transform.position;
                ParticleSystem.MainModule mainA = portalA.GetComponent<ParticleSystem>().main;
                ParticleSystem.MainModule mainB = portalB.GetComponent<ParticleSystem>().main;
                mainA.startColor = Color.red;
                mainB.startColor = Color.red;
                isTeleported = true;
            }
        }

        Collider[] collidersB = Physics.OverlapSphere(portalB.transform.position, 0.4f);

        foreach (var collider in collidersB)
        {
            if(collider.CompareTag("Player") && !isTeleported) {
                collider.transform.position = portalA.transform.position;
                ParticleSystem.MainModule mainA = portalA.GetComponent<ParticleSystem>().main;
                ParticleSystem.MainModule mainB = portalB.GetComponent<ParticleSystem>().main;
                mainA.startColor = Color.red;
                mainB.startColor = Color.red;
                isTeleported = true;
            }
        }
    }
}
