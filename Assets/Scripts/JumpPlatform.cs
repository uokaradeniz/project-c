using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    public float maxDist;
    bool playerJumped;

    void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            var playerComp = other.GetComponent<FPController>();
            playerComp.velocity.y += Mathf.Sqrt(maxDist - 2 * playerComp.gravity);
            if (!playerJumped)
                playerComp.totalJumps++;
            playerJumped = true;
            Invoke("ResetStatus", 0.5f);
        }
    }

    void ResetStatus() {
        playerJumped = false;
    }
}
