using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZiplineMechanics : MonoBehaviour
{
    Transform zipPoint1;
    Transform zipPoint2;
    Vector3 posChange;
    public float zipSlideSpeed;
    public bool hookP1;
    public bool hookP2;
    public bool traversed;
    float hookTimer;
    LineRenderer line;
    GameObject player;
    Vector3 translation;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        line = GetComponent<LineRenderer>();
        zipPoint1 = transform.Find("ZipPoint1");
        zipPoint2 = transform.Find("ZipPoint2");
    }

    // Update is called once per frame
    void Update()
    {
        if(traversed) {
            hookP1 = false;
            hookP2 = false;
            hookTimer += Time.deltaTime;
            if(hookTimer >= 2) {
                traversed = false;
                hookTimer = 0;
            }
        }

        zipPoint1.transform.LookAt(zipPoint2);
        zipPoint2.transform.LookAt(zipPoint1);
        line.SetPosition(0, zipPoint1.transform.Find("ZiplineHolder").position);
        line.SetPosition(1, zipPoint2.transform.Find("ZiplineHolder").position);

        Collider[] collidersP1 = Physics.OverlapSphere(zipPoint1.position, .1f);
        foreach (var collider in collidersP1)
        {
            if(collider.CompareTag("Player"))
                hookP1 = true;
        }

        Collider[] collidersP2 = Physics.OverlapSphere(zipPoint2.position, .1f);
        foreach (var collider in collidersP2)
        {
            if(collider.CompareTag("Player"))
                hookP2 = true;
        }

        if(hookP1 && !hookP2 && !traversed) {
            translation = Vector3.MoveTowards(player.transform.position, zipPoint2.position, zipSlideSpeed * Time.deltaTime);
            player.GetComponent<FPController>().movementRestricted = true;
            player.transform.position = translation;
        } else if(hookP2 && !hookP1 && !traversed) {
            translation = Vector3.MoveTowards(player.transform.position, zipPoint1.position, zipSlideSpeed * Time.deltaTime);
            player.GetComponent<FPController>().movementRestricted = true;
            player.transform.position = translation;
        } else if(hookP1 && hookP2) {
            player.GetComponent<FPController>().movementRestricted = false;
            traversed = true;
        }
    }
}
