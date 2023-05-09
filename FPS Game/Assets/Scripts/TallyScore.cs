using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Windows;
using Unity.VisualScripting;

public class TallyScore : MonoBehaviour, IDataPersistence
{
    [SerializeField] TMP_Text EnemiesKilled;
    [SerializeField] TMP_Text TimeSpent;
    [SerializeField] TMP_Text SecretsFound;
    [SerializeField] TMP_Text Deaths;
    [SerializeField] GameObject SceneLoader;
    [SerializeField] GameObject ContinueText;
    int enemiesKilled;
    int timeSpent;
    int secretsFound;
    int deaths;
    int temp;
    bool updating;

    private void Update()
    {
        if (!updating)
        {
            if (EnemiesKilled.text == "")
            {
                StartCoroutine(countText(EnemiesKilled, enemiesKilled));
            }
            else if (TimeSpent.text == "")
            {
                StartCoroutine(countTime());
            }
            else if (SecretsFound.text == "")
            {
                StartCoroutine(countText(SecretsFound, secretsFound));
            }
            else if (Deaths.text == "")
            {
                StartCoroutine(countText(Deaths, deaths));
            }
            else 
            {
                ContinueText.SetActive(true);
                ContinueText.transform.localPosition = new Vector3(0,Mathf.Sin(Time.time * 5),0) * 10;
                if (UnityEngine.Input.GetButtonDown("Continue"))
                    SceneLoader.SetActive(true);
            }
        }
    }

    IEnumerator countText(TMP_Text text, int number)
    {
        updating = true;
        if (number == 0)
            temp = 0;
        else
            temp = 1;
        while (text.text != number.ToString())
        {
            if (UnityEngine.Input.GetButtonDown("Continue"))
                temp = number;
            text.text = temp.ToString();
            temp++;
            if (number == 0)
                yield return null;
            else
                yield return new WaitForSeconds(5/number);
        }
        updating = false;
    }

    IEnumerator countTime()
    {
        updating = true;
        temp = 0;
        int seconds = 0;
        int minutes = 0;
        int hours = 0;
        while (temp <= timeSpent)
        {
            if (UnityEngine.Input.GetButtonDown("Continue"))
                temp = timeSpent;
            seconds = temp % 60;
            minutes = ((temp - seconds) / 60) % 60;
            hours = (((temp - seconds) / 60) - minutes) / 60;
            if (hours < 24)
            {
                TimeSpent.text = $"{hours.ToString()}:{minutes.ToString()}:{seconds.ToString()}";
                temp += timeSpent / 3600;
                yield return null;
            }
            else
            {
                TimeSpent.text = $"{((hours - hours % 24) / 24).ToString()} days" + (hours % 24 != 0 ? $" and {hours % 24} hours" : "");
                temp += 3600;
                yield return new WaitForSeconds(0.5f);
            }
        }
        updating = false;
    }

    public void LoadData(GameData data)
    {
        enemiesKilled = data.enemiesKilled;
        timeSpent = (int)data.time;
        secretsFound = data.secretsFound;
        deaths = data.deaths;
}

    public void SaveData(ref GameData data)
    {
        data.enemiesKilled = enemiesKilled;
        data.time = timeSpent;
        data.secretsFound = secretsFound;
        data.deaths = deaths;
    }
}
