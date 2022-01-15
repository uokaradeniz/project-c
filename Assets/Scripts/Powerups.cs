using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerups : MonoBehaviour
{
    bool powerupUsed;
    ParticleSystem par;
    PlayerGUI playerGUI;

    public enum Powerup
    {
        Cloak,
        Haste
    }

    public Powerup powerup;
    GameObject player;


    void Start()
    {
        playerGUI = GameObject.Find("Canvas").GetComponent<PlayerGUI>();
        player = GameObject.FindGameObjectWithTag("Player");
        par = GetComponent<ParticleSystem>();
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            par.Stop();
            GameObject.Destroy(gameObject, 10.5f);
            if (!powerupUsed)
            {
                switch (powerup)
                {
                    case Powerup.Cloak:
                        player.GetComponent<PlayerCombat>().playerInvisible = true;
                        playerGUI.cloakText.enabled = true;
                        Invoke("CloakStop", 10);
                        break;

                    case Powerup.Haste:
                        player.GetComponent<FPController>().runSpeed = player.GetComponent<FPController>().runSpeed + 3;
                        player.GetComponent<FPController>().duckSpeed = player.GetComponent<FPController>().duckSpeed + 2;
                        playerGUI.hasteText.enabled = true;
                        Invoke("HasteStop", 10);
                        break;
                }
            }
            powerupUsed = true;
        }
    }

    void CloakStop()
    {
        player.GetComponent<PlayerCombat>().playerInvisible = false;
        playerGUI.cloakText.enabled = false;
    }

    void HasteStop()
    {
        player.GetComponent<FPController>().runSpeed = player.GetComponent<FPController>().runSpeed - 3;
        player.GetComponent<FPController>().duckSpeed = player.GetComponent<FPController>().duckSpeed - 2;
        playerGUI.hasteText.enabled = false;
    }
}
