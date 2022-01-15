using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGUI : MonoBehaviour
{
    TextMeshProUGUI healthText;
    TextMeshProUGUI remainingBulletsText;
    TextMeshProUGUI diedText;
    TextMeshProUGUI winText;
    GameObject reloadText;
    GameObject btReadyText;
    [HideInInspector] public TextMeshProUGUI cloakText;
    [HideInInspector] public TextMeshProUGUI hasteText;
    Slider bulletTimeTimer;
    Image dynCrosshair;
    PlayerCombat player;
    BulletTimeMode bulletTimeMode;

    // Start is called before the first frame update
    void Start()
    {
        cloakText = GameObject.Find("PlayerHUD/CloakText").GetComponent<TextMeshProUGUI>();
        hasteText = GameObject.Find("PlayerHUD/HasteText").GetComponent<TextMeshProUGUI>();
        bulletTimeMode = GameObject.Find("Game Settings").GetComponent<BulletTimeMode>();
        bulletTimeTimer = transform.Find("PlayerHUD/BulletTimeTimer").GetComponent<Slider>();
        btReadyText = transform.Find("PlayerHUD/BTReadyText").gameObject;
        dynCrosshair = transform.Find("PlayerHUD/StaticCrosshair/DynamicCrosshair").GetComponent<Image>();
        remainingBulletsText = transform.Find("PlayerHUD/BulletText").GetComponent<TextMeshProUGUI>();
        reloadText = transform.Find("PlayerHUD/ReloadText").gameObject;
        player = GameObject.Find("Player").GetComponent<PlayerCombat>();
        healthText = transform.Find("PlayerHUD/HealthText").GetComponent<TextMeshProUGUI>();
        diedText = transform.Find("DiedText").GetComponent<TextMeshProUGUI>();
        winText = transform.Find("WinText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.playerDied)
        {
            diedText.enabled = true;
            healthText.text = "Health: " + "ERR";
            remainingBulletsText.text = "ERR";
            dynCrosshair.enabled = false;
            dynCrosshair.transform.parent.GetComponent<Image>().enabled = false;
        }
        else
        {
            if (player.GetComponent<PlayerCombat>().gameSettings.levelCleared)
            {
                winText.enabled = true;
                dynCrosshair.enabled = false;
                dynCrosshair.transform.parent.GetComponent<Image>().enabled = false;
            }

            healthText.text = "Health: " + player.playerHealth.ToString();
            if (bulletTimeMode.btActive)
            {
                bulletTimeTimer.maxValue = 3;
                bulletTimeTimer.value = bulletTimeMode.bulletTimer;
            }
            else if (bulletTimeMode.btCooldown)
            {
                bulletTimeTimer.maxValue = 8;
                bulletTimeTimer.value = bulletTimeMode.cdrTimer;
            }

            if (!bulletTimeMode.btCooldown)
                btReadyText.SetActive(true);
            else
                btReadyText.SetActive(false);

            if (bulletTimeMode.btActive)
                btReadyText.SetActive(false);

            RaycastHit hit;
            switch (player.selectedWeapon)
            {
                case 1:
                    if (player.isReloading)
                        dynCrosshair.enabled = false;
                    else
                        dynCrosshair.enabled = true;

                    if (player.remainingBulletsPistol <= 0)
                    {
                        reloadText.GetComponent<TextMeshProUGUI>().text = "R | Reload";
                        reloadText.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2 (226, 60);
                        reloadText.SetActive(true);
                    }
                    else
                        reloadText.SetActive(false);
                    var muzzleP = player.transform.Find("Main Camera/WeaponInventory/EnergyPistol/Muzzle");
                    if (Physics.Raycast(muzzleP.transform.position, muzzleP.transform.forward, out hit, Mathf.Infinity))
                        dynCrosshair.rectTransform.position = Camera.main.WorldToScreenPoint(hit.point);
                    else
                        dynCrosshair.rectTransform.anchoredPosition = new Vector2(0, 0);
                    remainingBulletsText.text = Mathf.RoundToInt(player.remainingBulletsPistol).ToString();
                    break;
                case 2:
                    if (player.isReloading)
                        dynCrosshair.enabled = false;
                    else
                        dynCrosshair.enabled = true;

                    if (player.remainingBulletsRifle <= 0)
                    {
                        reloadText.GetComponent<TextMeshProUGUI>().text = "R | Reload";
                        reloadText.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2 (226, 60);
                        reloadText.SetActive(true);
                    }
                    else
                        reloadText.SetActive(false);
                    var muzzleR = player.transform.Find("Main Camera/WeaponInventory/EnergyRifle/Muzzle");
                    if (Physics.Raycast(muzzleR.transform.position, muzzleR.transform.forward, out hit, Mathf.Infinity))
                        dynCrosshair.rectTransform.position = Camera.main.WorldToScreenPoint(hit.point);
                    else
                        dynCrosshair.rectTransform.anchoredPosition = new Vector2(0, 0);
                    remainingBulletsText.text = player.remainingBulletsRifle.ToString();
                    break;
                default:
                    remainingBulletsText.text = "N/A";
                    break;
                case 3:
                    reloadText.SetActive(false);
                    remainingBulletsText.text = "N/A";
                    dynCrosshair.enabled = false;
                    break;
                case 4:
                    dynCrosshair.enabled = true;
                    if (player.remainingBulletsPC <= 0)
                    {
                        reloadText.GetComponent<TextMeshProUGUI>().text = "Collect Charges";
                        reloadText.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2 (317, 60);
                        reloadText.SetActive(true);
                    }
                    else
                        reloadText.SetActive(false);
                    var muzzlePC = player.transform.Find("Main Camera/WeaponInventory/PlasmaCannon/Muzzle");
                    if (Physics.Raycast(muzzlePC.transform.position, muzzlePC.transform.forward, out hit, Mathf.Infinity))
                        dynCrosshair.rectTransform.position = Camera.main.WorldToScreenPoint(hit.point);
                    else
                        dynCrosshair.rectTransform.anchoredPosition = new Vector2(0, 0);
                    remainingBulletsText.text = player.remainingBulletsPC.ToString();
                    break;
            }
        }
    }
}
