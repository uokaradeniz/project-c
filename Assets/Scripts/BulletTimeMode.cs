using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeMode : MonoBehaviour
{
    float bulletTime = 3;
    [HideInInspector] public float bulletTimer;
    [HideInInspector] public float cdrTimer;

    [HideInInspector] public bool btActive;
    [HideInInspector] public bool btCooldown;

    // Start is called before the first frame update
    void Start()
    {
        bulletTimer = bulletTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Bullet Time") && !btActive && !btCooldown)
            btActive = true;

        if (btActive && !btCooldown)
        {
            Time.timeScale = 0.4f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            bulletTimer -= Time.deltaTime;

            if (bulletTimer <= 0 && btActive)
            {
                StopBulletTime();
                bulletTimer = bulletTime;
            }
        }
        if (btCooldown)
        {
            cdrTimer += Time.deltaTime;
            if (cdrTimer >= 8)
            {
                btCooldown = false;
                cdrTimer = 0;
            }
        }
    }

    public void StopBulletTime()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        btActive = false;
        btCooldown = true;
    }
}
