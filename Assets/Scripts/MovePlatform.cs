using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public GameObject[] waypoints;
    int current = 0;
    public float platformSpeed;

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, waypoints[current].transform.position) < 0.5) {
            current++;
            if(current >= waypoints.Length)
                current = 0;
        }
        transform.position = Vector3.MoveTowards(transform.position, waypoints[current].transform.position, platformSpeed * Time.deltaTime);
    }

    void OnTriggerEnter (Collider other) {
        if(other.CompareTag("Player")) {
            other.transform.parent = transform;
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player")) {
            other.transform.parent = null;
        }
    }
}
