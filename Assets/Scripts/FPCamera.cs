using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCamera : MonoBehaviour
{
    public float sensitivity;
    float rotationX;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!player.GetComponent<PlayerCombat>().playerDied && !GameObject.Find("Game Settings").GetComponent<GameSettings>().levelCleared)
        {
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90, 90);

            transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            player.transform.Rotate(Vector3.up * mouseX);
        }
    }
}
