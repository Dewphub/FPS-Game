using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("----- Player Information -----")]
    public GameObject player;
    public GameObject playerHead;
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
    public TextMeshProUGUI ammoPanelTMP;

    [Header("Passthrough Objects")]
    public LevelLoader levelLoader;
    public AudioSource curMusic;

    public bool isPaused;
    public bool playerIsAiming;
    public int enemiesRemaining = 0;
    public int enemiesKilled;

    public bool fadeIn;
    public bool fadeOut;
   
    float TIME_SCALE_DEFAULT;

    public static event Action PlayerHasDied;
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
        playerHead = GameObject.FindGameObjectWithTag("PlayerHead");
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        TIME_SCALE_DEFAULT = Time.timeScale;
        aim = player.GetComponentInChildren<Aim>();
        if(playerScript.gunList.Count > 0)
        {
            UpdateGunUI(playerScript.GetSelectedGunIndex(), playerScript.GetSelectedGun());
        }
        else
        {
            ammoPanelTMP.text = "";
        }
        HPBarColorHealthy = Color.green;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            aim.enabled = false;
            isPaused = !isPaused;
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);

            if (isPaused)
            {
                PauseState();
            }
            else
            {
                aim.enabled=true;
                ResumeState();
            }
        }
        if (fadeIn)
        {
            if (dyingIndicator.color.a < 1)
            {
                dyingIndicator.color += new Color(0f, 0f, 0f, 0.7f) * Time.deltaTime;
                if (dyingIndicator.color.a >= 0.99)
                {
                    fadeIn = false;
                    HideDyingIndicator();
                }
            }
        }
        if (fadeOut || (HPBar.fillAmount > 0.25f && dyingIndicator.color.a >= 0))
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

    void BossHasDied()
    {
        DefenderBossAI.Dying -= BossHasDied;
        
        /*activeMenu = winMenu;
        activeMenu.SetActive(true);
        PauseState();*/
        LoadNextLevelAsynchronously();
        DataPersistenceManager.Instance.SaveGame();
    }

    public void HideDyingIndicator()
    {
        fadeOut = true;
    }

    public void ShowDyingIndicator()
    {
        fadeIn = true;
    }

    public void PauseState()
    {
        reticle.SetActive(false);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeState()
    {
        reticle.SetActive(true);
        aim.enabled = true;
        Time.timeScale = TIME_SCALE_DEFAULT;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu = null;
    }

    public void UpdateGameGoal(int amount)
    {
        enemiesRemaining += amount;
        Debug.Log("Enemies = " + enemiesRemaining);/*
        if (amount < 0)
        {
            enemiesKilled -= amount;
        }*/
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
        PlayerHasDied?.Invoke();
        DataPersistenceManager.Instance.ModifyDeaths(1);
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
                ammoPanelTMP.text = gun.GetRemainingClipAmount().ToString() + " / " + gun.GetRemainingAmmo().ToString();
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

    public void LoadNextLevelAsynchronously()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        Debug.Log(sceneIndex);
        levelLoader.LoadNextScene(sceneIndex);
    }

    public void LoadPreviousLevelAsynchronously()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        Debug.Log(sceneIndex);
        levelLoader.LoadNextScene(sceneIndex);
    }

    public void ReloadLevelAsynchronously()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        levelLoader.LoadNextScene(sceneIndex);
    }

    public void BossHasSpawned()
    {
        curMusic.mute = true;
        DefenderBossAI.Dying += BossHasDied;
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public Vector3 GetGrenadeTargetPos()
    {
        return Vector3.ProjectOnPlane(player.transform.position, new(0, 1, 0));
    }
}
