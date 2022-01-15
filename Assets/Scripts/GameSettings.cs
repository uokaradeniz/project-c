using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    GameObject[] enemies;
    public bool levelCleared;

    GameObject mainMenu;
    GameObject optionsMenu;

    void Awake() {
        if(SceneManager.GetActiveScene().buildIndex == 1) {
            mainMenu = GameObject.Find("Panel");
            optionsMenu = GameObject.Find("OptionsPanel");
            optionsMenu.SetActive(false);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                GetComponent<BulletTimeMode>().StopBulletTime();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            }

            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if(enemies.Length == 0) {
                levelCleared = true;
            }
        }
    }

    public void StartButton()
    {
        GetComponent<BulletTimeMode>().StopBulletTime();
        SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
    }

    public void OptionsButton()
    {
        mainMenu.SetActive(false);
        optionsMenu.gameObject.SetActive(true);        
    }

    public void BackButton()
    {
        mainMenu.gameObject.SetActive(true);
        optionsMenu.gameObject.SetActive(false);        
    }


    public void QuitGameButtton()
    {
        Application.Quit();
    }

    public void CommercialsToMainMenu() {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
