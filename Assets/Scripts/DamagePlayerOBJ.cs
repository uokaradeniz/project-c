using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerOBJ : MonoBehaviour
{
    float damageTimer;
    public float damageInterval;

    void OnTriggerStay(Collider other) {
        if(other.CompareTag("Player") && !other.GetComponent<PlayerCombat>().playerDied) {
            damageTimer += Time.deltaTime;
            if(damageTimer >= damageInterval) {
            other.GetComponent<PlayerCombat>().playerHealth -= Random.Range(10, 30);
            damageTimer = 0;
            }
        }
    }
}
