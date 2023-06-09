using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<gunStats> gunList;
    public int selectedGun;
    public Vector3 playerPos;
    public SerializableDictionary<string, bool> gunsPickedUp;
    public SerializableDictionary<string, bool> turretsDestroyed;
    public int enemiesKilled;
    public float time;
    public int secretsFound;
    public int deaths;
    public Vector3 aimPos;

    public GameData()
    {
        this.gunList = new List<gunStats>();
        selectedGun = 0;
        playerPos = Vector3.zero;
        gunsPickedUp = new SerializableDictionary<string, bool>();
        aimPos = Vector3.zero;
        enemiesKilled = 0;
        time = 0;
        secretsFound = 0;
        deaths = 0;
    }
}
