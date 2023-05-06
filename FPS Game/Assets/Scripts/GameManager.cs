using System;
using System.Collections.Generic;
using TMPro;
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
    public Image HPBar;
    public Image towerHPBar;
    public TextMeshProUGUI enemiesRemainingText;

    public bool isPaused;
    public bool playerIsAiming;
    public int enemiesRemaining;

   
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
}
