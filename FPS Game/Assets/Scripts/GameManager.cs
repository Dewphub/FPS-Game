using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("----- Player Information -----")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerSpawnPos;
    public Aim aim;

    [Header("----- UI Information -----")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public GameObject checkpointMenu;
    public GameObject reticle;
    public Image HPBar;
    public Color HPBarColorHealthy;
    public Image towerHPBar;
    public Image dyingIndicator;
    public TextMeshProUGUI enemiesRemainingText;
    public RawImage previousGun;
    public RawImage nextGun;
    public RawImage currGun;
    public Texture blankGun;

    public bool isPaused;
    public bool playerIsAiming;
    public int enemiesRemaining;

    public bool fadeIn;
    public bool fadeOut;
   
    float TIME_SCALE_DEFAULT;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        TIME_SCALE_DEFAULT = Time.timeScale;
        aim = player.GetComponentInChildren<Aim>();
        if(playerScript.gunList.Count > 0)
        {
            UpdateGunUI(playerScript.GetSelectedGun(), playerScript.gunList[playerScript.GetSelectedGun()]);
        }
        HPBarColorHealthy = Color.green;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            isPaused = !isPaused;
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);

            if (isPaused)
            {
                PauseState();
            }
            else
            {
                ResumeState();
            }
        }
        if (fadeIn)
        {
            if (dyingIndicator.color.a < 1)
            {
                dyingIndicator.color += new Color(0f, 0f, 0f, 0.7f) * Time.deltaTime;
                if (dyingIndicator.color.a >= 1)
                {
                    fadeIn = false;
                    HideDyingIndicator();
                }
            }
        }
        if (fadeOut)
        {
            if (dyingIndicator.color.a >= 0)
            {
                dyingIndicator.color -= new Color(0f, 0f, 0f, 0.7f) * Time.deltaTime;
                if (dyingIndicator.color.a <= Mathf.Epsilon)
                {
                    fadeOut = false;
                    if(HPBar.fillAmount <= 0.25f)
                    {
                        ShowDyingIndicator();
                    }
                }
            }
        }
    }

    private void HideDyingIndicator()
    {
        fadeOut = true;
    }

    public void ShowDyingIndicator()
    {
        fadeIn = true;
    }

    public void PauseState()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeState()
    {
        Time.timeScale = TIME_SCALE_DEFAULT;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu = null;
    }

    public void UpdateGameGoal(int amount)
    {
        enemiesRemaining += amount;
        Debug.Log("Enemies Remaining: " + enemiesRemaining);
        //enemiesRemainingText.text = enemiesRemaining.ToString("F0");
/*        if (enemiesRemaining <= 0)
        {
            activeMenu = winMenu;
            activeMenu.SetActive(true);
            PauseState();
        }*/
    }

    public void OnDead()
    {
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
        PauseState();
    }

    public void TowerDestroyed()
    {
        activeMenu = winMenu;
        activeMenu.SetActive(true);
        PauseState();
    }

    public int GetEnemiesRemaining()
    {
        return enemiesRemaining;
    }

    public bool PlayerIsAiming() => aim.GetIsAiming();

    public void UpdateGunUI(int currentIndex, gunStats gun)
    {

        if (playerScript != null)
        {
            if (playerScript.gunList.Count > 0)
            {
                SetCurrentGun(gun);

                if (currentIndex > 0)
                {
                    gunStats prevGun = playerScript.gunList[currentIndex - 1];
                    SetDownGun(prevGun);
                }
                else
                {
                    ClearPrevGun();
                }

                if (currentIndex != playerScript.gunList.Count - 1)
                {
                    gunStats nextGun = playerScript.gunList[(currentIndex + 1)];
                    SetUpGun(nextGun);
                }
                else
                {
                    ClearNextGun();
                }
            }
            else
            {
                ClearPrevGun();
                ClearCurrentGun();
                ClearNextGun();
            }
        }
    }

    private void SetDownGun(gunStats _gun)
    {
        previousGun.enabled = true;
        previousGun.texture = _gun.gunIcon;
    }
    private void SetCurrentGun(gunStats _gun)
    {
        currGun.enabled = true;
        currGun.texture = _gun.gunIcon;
    }
    private void SetUpGun(gunStats _gun)
    {
        nextGun.enabled = true;
        nextGun.texture = _gun.gunIcon;
    }
    public void ClearPrevGun()
    {
        previousGun.texture = blankGun;
    }
    public void ClearCurrentGun()
    {
        currGun.texture = blankGun;
    }
    public void ClearNextGun()
    {
        nextGun.texture = blankGun;
    }

    public GameObject GetReticle()
    {
        return reticle;
    }
}
